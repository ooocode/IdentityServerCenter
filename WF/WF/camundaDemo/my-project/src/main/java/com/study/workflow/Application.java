package com.study.workflow;

import java.io.IOException;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;

@SpringBootApplication
public class Application {

  public static void main(String[] args) {

    // try{
    // var gRpcServer = NettyServerBuilder.forPort(9999).addService((new
    // GreeterService()).)
    // .addService(RouteGuideGrpc.bindService(new RouteGuideService(features)))
    // .build().start();
    /// } catch (IOException e) {
    // TODO Auto-generated catch block
    // e.printStackTrace();
    // }

    SpringApplication.run(Application.class);
  }

}