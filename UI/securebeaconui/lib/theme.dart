import 'package:flutter/material.dart';

final ThemeData appTheme = ThemeData(
  brightness: Brightness.dark,
  primaryColor: const Color(0xFF4B3BA6),
  scaffoldBackgroundColor: const Color(0xFF251E49),
  fontFamily: 'Roboto',

  colorScheme: ColorScheme.dark(
    primary: const Color(0xFF4B3BA6),
    secondary: const Color(0xFFA08CFF),
    background: const Color(0xFF251E49),
    surface: const Color(0xFF372D71),
    onPrimary: Colors.white,
    onSecondary: Colors.white,
    onSurface: Colors.white70,
  ),

  elevatedButtonTheme: ElevatedButtonThemeData(
    style: ElevatedButton.styleFrom(
      backgroundColor: const Color(0xFFA08CFF),
      foregroundColor: Colors.white,
      textStyle: const TextStyle(fontWeight: FontWeight.bold),
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(8),
      ),
    ),
  ),

  textTheme: const TextTheme(
    headlineLarge: TextStyle(
      color: Colors.white,
      fontSize: 28,
      fontWeight: FontWeight.bold,
    ),
    titleMedium: TextStyle(
      color: Colors.white70,
      fontSize: 18,
    ),
    bodyMedium: TextStyle(
      color: Colors.white70,
      fontSize: 16,
    ),
    labelLarge: TextStyle(
      color: Colors.white,
      fontWeight: FontWeight.w500,
    ),
  ),

  cardColor: const Color(0xFF372D71),
  dividerColor: Colors.white24,
  inputDecorationTheme: InputDecorationTheme(
    filled: true,
    fillColor: const Color(0xFF372D71),
    border: OutlineInputBorder(
      borderRadius: BorderRadius.circular(6),
    ),
    focusedBorder: OutlineInputBorder(
      borderSide: const BorderSide(color: Color(0xFFA08CFF)),
      borderRadius: BorderRadius.circular(6),
    ),
    labelStyle: const TextStyle(color: Colors.white),
  ),
);