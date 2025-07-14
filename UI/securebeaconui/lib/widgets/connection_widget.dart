import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../models/connection_status.dart';
import '../providers/connection_state_notifier.dart';

class ConnectionWidget extends ConsumerStatefulWidget {
  const ConnectionWidget({super.key});

  @override ConsumerState<ConnectionWidget> createState() =>
      _ConnectionWidgetState();
}

class _ConnectionWidgetState extends ConsumerState<ConnectionWidget> {
  late final _addressController;
  late final _portController;

  @override
  void initState()
  {
    super.initState();
    _addressController = TextEditingController(text: "https://httpbin.org/post") ; //"www.google.com/search");
    _portController = TextEditingController(text: "443");
  }

  @override
  void dispose()
  {
    _addressController.dispose();
    _portController.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final status = ref.watch(connectionStatusProvider);
    final isSending = (status != ConnectionStatus.stopped) && (status != ConnectionStatus.failed);
    final notifier = ref.read(connectionStatusProvider.notifier);

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
