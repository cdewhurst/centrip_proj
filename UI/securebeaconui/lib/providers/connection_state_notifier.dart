import 'dart:async';

import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../models/connection_status.dart';
import '../models/request_message.dart';
import '../services/tcp_client.dart';
import 'log_provider.dart';

class ConnectionStateNotifier extends StateNotifier<ConnectionStatus> {
  final Ref ref;
  late final _tcpClient;
  late final StreamSubscription<Map<String, dynamic>>? _subscription;

  ConnectionStateNotifier(this.ref) : super(ConnectionStatus.stopped)
  {
    _tcpClient = ref.read(tcpClientProvider);
    _subscription = _tcpClient.messages.listen((msg)
    {

    });
  }

  void toggleConnection(String address, int port) {
    if (state == ConnectionStatus.stopped) {
      // Currently stopped. Try to start
      ref.read(logProvider.notifier).add("Sent start request at ${DateTime.now()}");
      state = ConnectionStatus.startRequested;
      _tcpClient.send(RequestMessage(Command.start, address: address, port: port).toJsonString());
    } else {
      // Currently attempting connections. Try to stop
      ref.read(logProvider.notifier).add("Sent stop request at ${DateTime.now()}");
      state = ConnectionStatus.stopped;
      _tcpClient.send(RequestMessage(Command.stop).toJsonString());
    }
  }
}

final connectionStatusProvider =
StateNotifierProvider<ConnectionStateNotifier, ConnectionStatus>(
      (ref) => ConnectionStateNotifier(ref)
);
