using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCore
{
    public class RecvBuffer
    {
        ArraySegment<byte> buffer;

        //[ ][ ][ ][r][ ][ ][w ][ ][ ][ ]
        ArraySegment<byte> _buffer
        {
            get
            {
                return buffer;
            }
            set
            {
                buffer = value; 
            }
        }
        int _readPos;
        int _writePos;
        public RecvBuffer(int bufferSize) //생성자
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }
        
        public int DataSize { get { return _writePos - _readPos; } }// 유효범위(처리되지 않은 데이터의 사이즈)
        public int FreeSize { get { return _buffer.Count - _writePos; } }// 버퍼의 남은공간

        public ArraySegment<byte> ReadSegment// r부터 w이전 까지
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize); }
        }

        public ArraySegment<byte> WriteSegment//w 부터 끝까지
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize); }
        }

        //rw의 위치를 처음으로 당김
        public void Clean()
        { 
            int dataSize = DataSize;
            if (dataSize == 0)// r w가 겹치는 경우[ ] [ ] [ ] [rw]
            {
                //남은 데이터가 없으면 복사하지 않고 커서 위치만 리셋
                _readPos = _writePos = 0;
            }
            else
            {
                // 남은 데이터가 있으면 시작위치로 복사
                //[ ][ ][ ][r][ ][ ][w ][ ][ ][ ]
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos,_buffer.Array, _buffer.Offset, dataSize);
                _readPos = 0;
                _writePos= dataSize;
            }
        }

        public bool OnRead(int numOfBytes)
        {
            //[ ][ ][ ][r][ ][ ][w ][ ][ ][ ]
            if (numOfBytes > DataSize)
                return false;
            //[ ][ ][ ][][ ][ ][rw][ ][ ][ ]
            _readPos += numOfBytes;
            return true;
        }

        public bool OnWrite(int numOfBytes)
        {
            if(numOfBytes > FreeSize) 
                return false;

            _writePos += numOfBytes;
            return true;

        }
    }
}
