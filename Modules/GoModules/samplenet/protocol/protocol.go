package protocol

import (
	"bytes"
	"encoding/binary"
)

//整型转化成字节
func Int2Bytes(n int) []byte {
	x := int32(n)
	bytesBuffer := bytes.NewBuffer([]byte{})
	binary.Write(bytesBuffer, binary.BigEndian, x)
	return bytesBuffer.Bytes()
}

//字节转化成整型
func Bytes2Int(b []byte) int {
	bytesBuffer := bytes.NewBuffer(b)
	var x int32
	binary.Read(bytesBuffer, binary.BigEndian, &x)
	return int(x)
}

func Uint16ToBytes(n uint16) []byte {
	return []byte{
		byte(n),
		byte(n >> 8),
	}
}

//获得消息长度 byte转int16
func BytesToUint16(array []byte) uint16 {
	var data uint16 = 0
	for i := 0; i < len(array); i++ {
		data = data + uint16(uint(array[i])<<uint(8*i))
	}
	return data
}
