using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;

namespace johnshope.Sync {

	enum StreamConsumer { Undefined, Reader, Writer }
 
	public class PipeStream: Stream {

		private object _lockForRead;
		private object _lockForAll;
		private Queue<object> _chunks;
		private object _currentChunk;
		private long _currentChunkPosition;
		private ManualResetEvent _doneWriting;
		private ManualResetEvent _dataAvailable;
		private WaitHandle[] _events;
		private int _doneWritingHandleIndex;
		private volatile bool _illegalToWrite;
		private Exception _exception;

		[ThreadStatic]
		private static StreamConsumer _consumer = StreamConsumer.Undefined;

		public PipeStream() {
			_chunks = new Queue<object>();
			_doneWriting = new ManualResetEvent(false);
			_dataAvailable = new ManualResetEvent(false);
			_events = new WaitHandle[] { _dataAvailable, _doneWriting };
			_doneWritingHandleIndex = 1;
			_lockForRead = new object();
			_lockForAll = new object();
		}

		public override bool CanRead { get { return true; } }
		public override bool CanSeek { get { return false; } }
		public override bool CanWrite { get { return !_illegalToWrite; } }

		public override void Flush() { }
		public override long Length {
			get { throw new NotSupportedException(); }
		}
		public override long Position {
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}
		public override long Seek(long offset, SeekOrigin origin) {
			throw new NotSupportedException();
		}
		public override void SetLength(long value) {
			throw new NotSupportedException();
		}

		public override int Read(byte[] buffer, int offset, int count) {
			if (_consumer == StreamConsumer.Writer) throw new NotSupportedException("You cannot read & write to a BlockingStream from the same thread."); 
			_consumer = StreamConsumer.Reader;

			lock (_lockForAll) if (_exception != null) { var ex = _exception; _exception = null; throw ex; }

			if (buffer == null) throw new ArgumentNullException("buffer");
			if (offset < 0 || offset >= buffer.Length)
				throw new ArgumentOutOfRangeException("offset");
			if (count < 0 || offset + count > buffer.Length)
				throw new ArgumentOutOfRangeException("count");
			if (_dataAvailable == null)
				throw new ObjectDisposedException(GetType().Name);

			if (count == 0) return 0;

			while (true) {
				int handleIndex = WaitHandle.WaitAny(_events);
				lock (_lockForRead) {
					lock (_lockForAll) {
						if (_currentChunk == null) {
							if (_chunks.Count == 0) {
								if (handleIndex == _doneWritingHandleIndex)
									return 0;
								else continue;
							}
							_currentChunk = _chunks.Dequeue();
							_currentChunkPosition = 0;
						}
					}

					if (_currentChunk is Stream) {
						var read = ((Stream)_currentChunk).Read(buffer, offset, count);
						if (read  != count) {
							_currentChunk = null;
							_currentChunkPosition = 0;
							lock (_lockForAll) {
								if (_chunks.Count == 0) _dataAvailable.Reset();
							}
						} else {
							_currentChunkPosition += count;
						}
						return read;

					} else {

						long bytesAvailable =
							((_currentChunk is Stream) ? ((Stream)_currentChunk).Length : ((byte[])_currentChunk).Length) - _currentChunkPosition;
						int bytesToCopy;
						if (bytesAvailable > count) {
							bytesToCopy = count;
							Buffer.BlockCopy((byte[])_currentChunk, (int)_currentChunkPosition, buffer, offset, count);
							_currentChunkPosition += count;
						} else {
							bytesToCopy = (int)bytesAvailable;
							Buffer.BlockCopy((byte[])_currentChunk, (int)_currentChunkPosition, buffer, offset, bytesToCopy);
							_currentChunk = null;
							_currentChunkPosition = 0;
							lock (_lockForAll) {
								if (_chunks.Count == 0) _dataAvailable.Reset();
							}
						}
						return bytesToCopy;
					}
				}
			}
		}

		public virtual void Read(Stream stream) {
			if (_consumer == StreamConsumer.Writer) throw new NotSupportedException("You cannot read & write to a BlockingStream from the same thread.");
			_consumer = StreamConsumer.Reader;

			if (_dataAvailable == null)
				throw new ObjectDisposedException(GetType().Name);

			lock (_lockForAll) if (_exception != null) { var ex = _exception; _exception = null; throw ex; }

			while (true) {
				int handleIndex = WaitHandle.WaitAny(_events);
				lock (_lockForRead) {
					lock (_lockForAll) {
						if (_currentChunk == null) {
							if (_chunks.Count == 0) {
								if (handleIndex == _doneWritingHandleIndex)
									return;
								else continue;
							}
							_currentChunk = _chunks.Dequeue();
							_currentChunkPosition = 0;
						}
					}

					if (_currentChunk is Stream) {
						Streams.Copy((Stream)_currentChunk, stream);
					} else {
						var buffer = (byte[])_currentChunk;
						stream.Write(buffer, (int)_currentChunkPosition, buffer.Length);
					}
					_currentChunk = null;
					_currentChunkPosition = 0;
					lock (_lockForAll) {
						if (_chunks.Count == 0) _dataAvailable.Reset();
					}
				}
			}
		}

		public override void Write(byte[] buffer, int offset, int count) {
			if (_consumer == StreamConsumer.Reader) throw new NotSupportedException("You cannot read & write to a BlockingStream from the same thread.");
			_consumer = StreamConsumer.Writer;

			lock(_lockForAll) if (_exception != null) { var ex = _exception; _exception = null; throw ex; }

			if (buffer == null) throw new ArgumentNullException("buffer");
			if (offset < 0 || offset >= buffer.Length)
				throw new ArgumentOutOfRangeException("offset");
			if (count < 0 || offset + count > buffer.Length)
				throw new ArgumentOutOfRangeException("count");
			if (_dataAvailable == null)
				throw new ObjectDisposedException(GetType().Name);

			if (count == 0) return;

			byte[] chunk = new byte[count];
			Buffer.BlockCopy(buffer, offset, chunk, 0, count);
			lock (_lockForAll) {
				if (_illegalToWrite)
					throw new InvalidOperationException(
						"Writing has already been completed.");
				
				_chunks.Enqueue(chunk);
				_dataAvailable.Set();
			}
		}

		public virtual void Write(Stream stream) {
			if (_consumer == StreamConsumer.Reader) throw new NotSupportedException("You cannot read & write to a BlockingStream from the same thread.");
			_consumer = StreamConsumer.Writer;

			if (_dataAvailable == null)
				throw new ObjectDisposedException(GetType().Name);

			lock (_lockForAll) {
				if (_exception != null) { var ex = _exception; _exception = null; throw ex; }
				if (_illegalToWrite)
					throw new InvalidOperationException(
						"Writing has already been completed.");

				_chunks.Enqueue(stream);
				_dataAvailable.Set();
			}
		}
 
		public void Exception(Exception ex) { lock(_lockForAll) _exception = ex; }

		public void SetEndOfStream() {
			if (_consumer == StreamConsumer.Reader) throw new NotSupportedException("You cannot read & write to a BlockingStream from the same thread.");
			if (_dataAvailable == null)
				throw new ObjectDisposedException(GetType().Name);
			lock (_lockForAll) {
				_consumer = StreamConsumer.Undefined;
				_illegalToWrite = true;
				_doneWriting.Set();
			}
		}

		public override void Close() {
			if (_consumer == StreamConsumer.Reader) {
				_consumer = StreamConsumer.Undefined;
				base.Close();
				if (_dataAvailable != null) {
					_dataAvailable.Close();
					_dataAvailable = null;
				}
				if (_doneWriting != null) {
					_doneWriting.Close();
					_doneWriting = null;
				}
			} else if (_consumer == StreamConsumer.Writer) {
				SetEndOfStream();
				//_consumer = StreamConsumer.Undefined;
			}
		}
	}
}