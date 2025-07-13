import 'dart:async';
import 'dart:convert';
import 'dart:io';
import 'dart:typed_data';

import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../providers/log_provider.dart';

class TcpClient {
  Socket? _socket;
  late final StreamController<Map<String, dynamic>> _inputStreamController;

  TcpClient() {
    _inputStreamController = StreamController<Map<String, dynamic>>.broadcast();
  }

  Future<void> connect(String host, int port) async {
      _socket = await Socket.connect(host, port);

      _socket!.listen((data) {
        try {
          final jsonString = utf8.decode(data);
          Map<String, dynamic> msg = jsonDecode(jsonString);
          _inputStreamController.add(msg);
        }
        catch (e) {
          print('Invalid JSON in socket message $e');
        }
      }, onError: (error) {
        _inputStreamController.addError(error);
      }, onDone: () {
        _inputStreamController.close();
      });
  }

  void send(String message) {
    if (_socket != null) {
      _socket?.write(message + '\n');
    }
  }

  void disconnect() {
    _socket?.destroy();
    _socket = null;
  }

  Stream<Map<String, dynamic>> get messages => _inputStreamController.stream;

  bool get isConnected => _socket != null;
}

final tcpClientProvider = Provider<TcpClient>((ref) {
  throw UnimplementedError(); // Should never be called as is overridden in main
});