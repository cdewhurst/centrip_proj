import 'dart:async';

import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../services/tcp_client.dart';

final logProvider = StateNotifierProvider<LogNotifier, List<String>>((ref) {
  return LogNotifier(ref);
});

class LogNotifier extends StateNotifier<List<String>> {
  late final StreamSubscription<Map<String, dynamic>>? _subscription;

  LogNotifier(ref) : super([])
  {
    final tcpClient = ref.read(tcpClientProvider);
    _subscription = tcpClient.messages.listen(
        (msg) {
          if (msg['Type'] == 'Log')
            {
              add(msg['Message']);
            }
        },
      onError: (e) {
        add('[ERROR] $e');
      },
      cancelOnError: false,
    );
  }

  void add(String message) {
    state = [...state, message];
  }

  void clear() {
    state = [];
  }
}