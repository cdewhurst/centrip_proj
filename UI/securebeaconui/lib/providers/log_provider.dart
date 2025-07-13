import 'package:flutter_riverpod/flutter_riverpod.dart';

final logProvider = StateNotifierProvider<LogNotifier, List<String>>((ref) {
  return LogNotifier();
});

class LogNotifier extends StateNotifier<List<String>> {
  LogNotifier() : super([]);

  void add(String message) {
    state = [...state, message];
  }

  void clear() {
    state = [];
  }
}