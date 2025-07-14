String nowInIso8601WithOffset() {
  DateTime dt = DateTime.now();
  final duration = dt.timeZoneOffset;
  final hours = duration.inHours.abs().toString().padLeft(2, '0');
  final minutes = (duration.inMinutes.abs() % 60).toString().padLeft(2, '0');
  final sign = duration.isNegative ? '-' : '+';
  final offset = '$sign$hours:$minutes';

  return dt.toIso8601String() + offset;
}