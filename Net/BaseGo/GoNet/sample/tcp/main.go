//////////////////////////////////////////////////////////////////////////////////////////
//// time:2019/3/25 下午2:20:17
//// author:BanMing
//// des:简单的tcp对应simple-tcp-go
////////////////////////////////////////////////////////////////////////////////////////////
package main

import (
	"GoNet/sample/protocol"
	"fmt"
	"net"
	"strings"
)

type TcpClient struct {
}

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
		go handleClientTest(conn)
	}
}

//分发数据
func handleClientTest(conn net.Conn) {
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
		//这里处理粘包的问题
		msgLength := protocol.BytesToUint16(buf[:HeaderLen])
		if cnt >= int(msgLength)+HeaderLen {
			//这里超多包长的直接弃用
			reciveData := buf[HeaderLen : HeaderLen+msgLength]
			//todo:抛出消息到逻辑层
			sendTest(conn, reciveData)
		} else {
			conn.Close()
			fmt.Println("msg length error")
		}
	}
}

//测试发送
func sendTest(conn net.Conn, reciveData []byte) {
	inStr := strings.TrimSpace(string(reciveData))
	fmt.Println("inStr:" + inStr)
	send := []byte("sever:" + inStr)
	sendLenB := protocol.Uint16ToBytes(uint16(len(send)))
	temp := append(sendLenB, send...)
	//fmt.Println("send length:" + strconv.Itoa(len(temp)))
	//fmt.Println(temp)
	conn.Write(temp)
}
