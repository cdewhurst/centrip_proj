import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../models/connection_status.dart';
import '../providers/connection_state_notifier.dart';

class ConnectionWidget extends ConsumerWidget {
  final _addressController = TextEditingController();
  final _portController = TextEditingController();

  ConnectionWidget({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final isSending = ref.watch(connectionStatusProvider) != ConnectionStatus.stopped;
    final notifier = ref.read(connectionStatusProvider.notifier);

    _addressController.text = "www.google.com/search";
    _portController.text = "443";

    return Padding(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Server Connection',
            style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 12),
          Row(
            children: [
              Expanded(
                child: TextField(
                  controller: _addressController,
                  decoration: const InputDecoration(
                    labelText: 'Server Address',
                    border: OutlineInputBorder(),
                  ),
                ),
              ),
              const SizedBox(width: 12),
              SizedBox(
                width: 100,
                child: TextField(
                  controller: _portController,
                  decoration: const InputDecoration(
                    labelText: 'Port',
                    border: OutlineInputBorder(),
                  ),
                ),
              ),
              const SizedBox(width: 12),
              ElevatedButton.icon(
                icon: Icon(isSending ? Icons.stop : Icons.play_arrow),
                label: Text(isSending ? 'Stop' : 'Start'),
                style: ElevatedButton.styleFrom(
                  backgroundColor: isSending ? Colors.red : Colors.green,
                ),
                onPressed: () {
                  final address = _addressController.text.trim();
                  final port = int.tryParse(_portController.text.trim());

                  // Simple validation
                  if (address.isEmpty || port == null || port < 0 || port > 65535) {
                    ScaffoldMessenger.of(context).showSnackBar(
                      const SnackBar(content: Text('Enter both server address and port as an integer.')),
                    );
                    return;
                  }

                  notifier.toggleConnection(address, port);
                },
              ),
            ],
          ),
        ],
      ),
    );
  }
}
