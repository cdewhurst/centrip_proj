import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'screens/main_screen.dart';
import 'services/tcp_client.dart';
import 'theme.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();

  final tcpClient = TcpClient();
  await tcpClient.connect('127.0.0.1', 30341); // Arbitrary port

  runApp(
      ProviderScope( // Sets up Riverpod usage.
        overrides: [
          tcpClientProvider.overrideWithValue(tcpClient)
        ],
        child: const MyApp()
    )
  );
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  // This widget is the root of your application.
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: null, // No title bar required inside a Windows app.
      theme: appTheme,
      home: const MainScreen(),
    );
  }
}