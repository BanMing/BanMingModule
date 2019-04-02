//////////////////////////////////////////////////////////////////////////////////////////
//// time:2019/3/25 下午2:20:17
//// author:BanMing
//// des:简单的tcp对应simple-tcp-go
////////////////////////////////////////////////////////////////////////////////////////////
package main

import (
	"fmt"
	"net"
	"os"
	"strings"
)

var MaxLen = 4096

func main() {
	service := ":7777"
	tcpAddr, err := net.ResolveTCPAddr("tcp4", service)
	checkError(err)
	listener, err := net.ListenTCP("tcp", tcpAddr)
	checkError(err)
	for {
		conn, err := listener.Accept()
		if err != nil {
			continue
		}
		go handleClient(conn)
	}
}

//分发数据
func handleClient(conn net.Conn) {
	if conn == nil {
		return
	}
	fmt.Println("One Connect~")
	buf := make([]byte, MaxLen)
	for {
		cnt, err := conn.Read(buf)
		//fmt.Println("cnt:" + strconv.Itoa(cnt))
		if err != nil || cnt == 0 {
			conn.Close()
			break
		}
		//消息长度
		msgLength := bytesToUint16(buf)
		if cnt >= int(msgLength)+2 {
			inStr := strings.TrimSpace(string(buf[2 : 2+msgLength]))
			fmt.Println("inStr:" + inStr)
			//todo:抛出消息到逻辑层

		} else {
		//
		}

		//fmt.Println(msgLength)

		//send := []byte("sever:" + inStr)

		//conn.Write()

	}
	//defer conn.Close()
	//daytime := time.Now().String()
	//conn.Write([]byte(daytime))
}

func checkError(err error) {
	if err != nil {
		fmt.Println(os.Stderr, "Fatal error:%s", err.Error())
		os.Exit(1)
	}
}

func Uint16ToBytes(n uint16) []byte {
	return []byte{
		byte(n),
		byte(n >> 8),
	}
}

//获得消息长度 byte转int16
func bytesToUint16(array []byte) uint16 {
	var data uint16 = 0
	for i := 0; i < len(array); i++ {
		data = data + uint16(uint(array[i])<<uint(8*i))
	}
	return data
}
