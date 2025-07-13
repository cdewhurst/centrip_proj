import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../models/connection_status.dart';
import 'log_provider.dart';

class ConnectionStateNotifier extends StateNotifier<ConnectionStatus> {
  final Ref ref;

  ConnectionStateNotifier(this.ref) : super(ConnectionStatus.stopped);

  String? _address;
  String? _port;

  void toggleConnection(String address, String port) {
    if (state == ConnectionStatus.stopped) {
      // Currently stopped. Try to start
      ref.read(logProvider.notifier).add("Sent start request at ${DateTime.now()}");
      _address = address;
      _port = port;
      state = ConnectionStatus.startRequested;
    } else {
      // Currently attempting connections. Try to stop
      ref.read(logProvider.notifier).add("Sent stop request at ${DateTime.now()}");
      state = ConnectionStatus.stopped;
    }
  }

  String? get address => _address;
  String? get port => _port;
}

final connectionStatusProvider =
StateNotifierProvider<ConnectionStateNotifier, ConnectionStatus>(
      (ref) => ConnectionStateNotifier(ref)
);
