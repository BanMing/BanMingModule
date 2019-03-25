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
	"time"
)

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
	fmt.Println("One Connect~")
	//defer conn.Close()
	daytime := time.Now().String()
	conn.Write([]byte(daytime))
}

func checkError(err error) {
	if err != nil {
		fmt.Println(os.Stderr, "Fatal error:%s", err.Error())
		os.Exit(1)
	}
}
