package main

import (
	"fmt"
	"net"
	"os"
)

const (
	HeaderLen = 2    //消息都长度
	MaxLen    = 1024 //消息最大长度
)

type SampleTcp struct {
	conn net.Conn
}

//开启一个tcp
func StartSampleTcp(prot string) {
	service := ":" + prot
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

func handleClient(conn net.Conn) {

}

func checkError(err error) {
	if err != nil {
		fmt.Println(os.Stderr, "Fatal error:%s", err.Error())
		os.Exit(1)
	}
}
