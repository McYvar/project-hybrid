#include "Strip.h"

Strip::Strip(int size, int16_t pin)
  : strip(Adafruit_NeoPixel(size, pin, NEO_GRB + NEO_KHZ800)), stripSize(size - 1) {}

void Strip::Initialize() {
  strip.begin();
  strip.clear();
  Update();
}

void Strip::Update() {
  strip.show();
}

uint16_t Strip::GetSize() {
  return stripSize;
}

uint32_t Strip::SetColor(uint8_t r, uint8_t g, uint8_t b) {
  return strip.Color(r, g, b);
}

void Strip::Fill(int start, int end, uint32_t color) {
  if (end <= start || start > stripSize) return;
  for (int i = start; i < end; i++) {
    strip.setPixelColor(i, color);
  }
}

void Strip::Fill(int start, int end, uint8_t r, uint8_t g, uint8_t b) {
  if (end <= start || start > stripSize) return;
  Fill(start, end, strip.Color(r, g, b));
}

void Strip::Spread(int start, int spreadSize, uint8_t r, uint8_t g, uint8_t b, int brightnessReductor) {
  if (start > stripSize) return;
  if (brightnessReductor < 1) brightnessReductor = 1;

  int reductorIterator = 0;
  for (int i = start; i < start + spreadSize; i++) {
    reductorIterator++;

    int u = i;
    if (u > stripSize) u -= stripSize;

    int d = start - (i - start);
    if (d < 0) d = stripSize + d;

    strip.setPixelColor(u,
                        r - (reductorIterator * brightnessReductor) > 0 ? r - (reductorIterator * brightnessReductor) : 0,
                        g - (reductorIterator * brightnessReductor) > 0 ? g - (reductorIterator * brightnessReductor) : 0,
                        b - (reductorIterator * brightnessReductor) > 0 ? b - (reductorIterator * brightnessReductor) : 0);
    strip.setPixelColor(d,
                        r - (reductorIterator * brightnessReductor) > 0 ? r - (reductorIterator * brightnessReductor) : 0,
                        g - (reductorIterator * brightnessReductor) > 0 ? g - (reductorIterator * brightnessReductor) : 0,
                        b - (reductorIterator * brightnessReductor) > 0 ? b - (reductorIterator * brightnessReductor) : 0);
  }
}

void Strip::Clear() {
  strip.clear();
}
