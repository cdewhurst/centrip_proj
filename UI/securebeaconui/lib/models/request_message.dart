import 'dart:convert';

enum Command {
  start,
  stop,
}

class RequestMessage {
  final Command command;
  final String? address;
  final int? port;

  RequestMessage(this.command, {this.address, this.port });

  String toJsonString() {
    if (this.address != null) {
      return jsonEncode({
        'command': command.name,
        'address': address,
        'port': port,
      });
    }
    else
      {
        return jsonEncode({
          'command': command.name,
        });
      }
  }
}