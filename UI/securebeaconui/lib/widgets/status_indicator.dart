import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:securebeaconui/providers/connection_state_notifier.dart';
import '../models/connection_status.dart';

class _StatusVisuals {
  final String label;
  final Color color;
  final IconData icon;

  _StatusVisuals(this.label, this.color, this.icon);
}

class StatusIndicator extends ConsumerStatefulWidget {
  const StatusIndicator({super.key});

  @override
  ConsumerState<StatusIndicator> createState() => _StatusIndicatorState();
}

class _StatusIndicatorState extends ConsumerState<StatusIndicator> {
  @override
  Widget build(BuildContext context) {
    final status = ref.watch(connectionStatusProvider);

    final statusData = _getStatusData(status);
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Icon(statusData.icon, color: statusData.color),
        const SizedBox(width: 8),
        Text(
          statusData.label,
          style: TextStyle(color: statusData.color, fontWeight: FontWeight.bold),
        ),
      ],
    );
  }

  _StatusVisuals _getStatusData(ConnectionStatus status) {
    switch (status) {
      case ConnectionStatus.lastCallGood:
        return _StatusVisuals("Last call returned 200 (OK)", Colors.green, Icons.check_circle);
      case ConnectionStatus.stopped:
        return _StatusVisuals("Connection stopped", Colors.grey, Icons.radio_button_unchecked);
      case ConnectionStatus.startRequested:
        return _StatusVisuals("Starting connection", Colors.grey, Icons.radio_button_unchecked);
      case ConnectionStatus.retrying:
        return _StatusVisuals("Retrying...", Colors.orange, Icons.autorenew);
      case ConnectionStatus.failed:
        return _StatusVisuals("Failed", Colors.red, Icons.error);
    }
  }
}