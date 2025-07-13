import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../widgets/connection_widget.dart';
import '../widgets/log_viewer.dart';
import '../widgets/status_indicator.dart';

class MainScreen extends ConsumerStatefulWidget {
  const MainScreen({super.key});

  @override
  ConsumerState<MainScreen> createState() => _MainScreenState();
}

class _MainScreenState extends ConsumerState<MainScreen> {
  final _addressController = TextEditingController();
  final _portController = TextEditingController(text: '443');

  @override
  void dispose() {
    _addressController.dispose();
    _portController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: null, // Windows window title bar takes care of this where we want only a title.
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            ConnectionWidget(),
            const SizedBox(height: 16),
            const StatusIndicator(),
            const SizedBox(height: 24),
            const LogViewer()
          ],
        ),
      ),
    );
  }
}
