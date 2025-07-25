import 'dart:async';

import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:securebeaconui/utils/iso8601.dart';

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
      if (msg['Type'] == 'Status')
      {
        state = ConnectionStatus.values.firstWhere(
              (e) => e.name.toLowerCase() == msg['Status'].toLowerCase(),
          orElse: () => ConnectionStatus.failed,
        );
      }
    });
  }

  void toggleConnection(String address, int port) {
    if (state == ConnectionStatus.stopped || state == ConnectionStatus.failed) {
      // Currently stopped. Try to start
      ref.read(logProvider.notifier).add("${nowInIso8601WithOffset()} Sent start request");
      _tcpClient.send(RequestMessage(Command.start, address: address, port: port).toJsonString());
    } else {
      // Currently attempting connections. Try to stop
      ref.read(logProvider.notifier).add("${nowInIso8601WithOffset()} Sent stop request");
      _tcpClient.send(RequestMessage(Command.stop).toJsonString());
    }
  }
}

final connectionStatusProvider =
StateNotifierProvider<ConnectionStateNotifier, ConnectionStatus>(
      (ref) => ConnectionStateNotifier(ref)
);
