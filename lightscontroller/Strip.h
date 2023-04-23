#include <Adafruit_NeoPixel.h>

class Strip {
  public:
    Strip(int size, int16_t pin);
    void Initialize();
    void Update();
    uint16_t GetSize();
    uint32_t SetColor(uint8_t r, uint8_t g, uint8_t b);

    void Fill(int start, int end, uint32_t color);
    void Fill(int start, int end, uint8_t r, uint8_t g, uint8_t b);
    void Spread(int start, int spreadSize, uint8_t r, uint8_t g, uint8_t b, int brightnessReductor);
    void Clear();

  private:
    Adafruit_NeoPixel strip;
    int stripSize;
};