#include "BluetoothSerial.h"
#include <SoftwareSerial.h>
#include "MPU9250.h"

// Quaterions
#define MPU9250_IMU_ADDRESS 0x68

//#define G 9.80665

#define MAGNETIC_DECLINATION 1.63  // To be defined by user
#define INTERVAL_MS_PRINT 34

MPU9250 mpu;

// Bluetooth
#if !defined(CONFIG_BT_ENABLED) || !defined(CONFIG_BLUEDROID_ENABLED)
#error Bluetooth is not enabled! Please run `make menuconfig` to and enable it
#endif

#if !defined(CONFIG_BT_SPP_ENABLED)
#error Serial Bluetooth not available or not enabled. It is only available for the ESP32 chip.
#endif

BluetoothSerial SerialBT;

unsigned long lastPrintMillis = 0;
unsigned long currentMillis = 0;

double gx = 0.0, gy = 0.0, gz = 0.0;
double deviationX = 0.0, deviationY = 0.0, deviationZ = 0.0;

double pitch = 0.0, roll = 0.0, yaw = 0.0;
double ax = 0.0, ay = 0.0, az = 0.0;

double deltaTime = 0.0;

void setup() {
  Serial.begin(115200);
  
  // Bluetooth
  SerialBT.begin("Regulator");  //Bluetooth device name
  Serial.println("The device started, now you can pair it with bluetooth!");
  
  // MPU-9250
  Wire.begin();

  MPU9250Setting setting;

  // Sample rate must be at least 2x DLPF rate
  setting.accel_fs_sel = ACCEL_FS_SEL::A16G;
  setting.gyro_fs_sel = GYRO_FS_SEL::G1000DPS;
  setting.mag_output_bits = MAG_OUTPUT_BITS::M16BITS;
  setting.fifo_sample_rate = FIFO_SAMPLE_RATE::SMPL_250HZ;
  setting.gyro_fchoice = 0x03;
  setting.gyro_dlpf_cfg = GYRO_DLPF_CFG::DLPF_20HZ;
  setting.accel_fchoice = 0x01;
  setting.accel_dlpf_cfg = ACCEL_DLPF_CFG::DLPF_45HZ;

  mpu.setup(MPU9250_IMU_ADDRESS, setting);

  mpu.setMagneticDeclination(MAGNETIC_DECLINATION);
  mpu.selectFilter(QuatFilterSel::MADGWICK);
  mpu.setFilterIterations(15);

  SerialBT.println("Calibrating...");
  mpu.calibrateAccelGyro();
}

void loop() {
  // Bluetooth
  String command = "";
  while (SerialBT.available()) {
    int read = SerialBT.read();
    if (read != 10 && read != 13) // cuz 10/13 === return
      command += (char)read;
  }
  
  if (command.length() > 0) {   // means there was something to read...
    if (command == "reInit") reInitHandler();
  }

  // MPU-9250
  deltaTime = (millis() - currentMillis) / 1000.0;
  currentMillis = millis();

  //gx = 0.98 * (mpu.getRoll() + mpu.getGyroX() * deltaTime) + 0.02 * mpu.getAccX();
  //gy = 0.98 * (mpu.getPitch() + mpu.getGyroY() * deltaTime) + 0.02 * mpu.getAccY();
  //gz = 0.98 * (mpu.getYaw() + mpu.getGyroZ() * deltaTime) + 0.02 * mpu.getAccZ();

  gx += (mpu.getGyroX() * deltaTime) - deviationX;
  gy -= (mpu.getGyroY() * deltaTime) - deviationY;
  gz -= (mpu.getGyroZ() * deltaTime) - deviationZ;

  if (gx < 0) gx = 360 + gx; if (gx > 360) gx = gx - 360;
  if (gy < 0) gy = 360 + gy; if (gy > 360) gy = gy - 360;
  if (gz < 0) gz = 360 + gz; if (gz > 360) gz = gz - 360;

  pitch = mpu.getPitch();
  roll = mpu.getRoll();
  yaw = mpu.getYaw();

  ax = mpu.getAccX();
  ay = mpu.getAccY();
  az = mpu.getAccZ();

  if (mpu.update() && currentMillis - lastPrintMillis > INTERVAL_MS_PRINT) {
    // MPU output has to be translated to a string readable by unity
    SerialBT.print(gx, 5);
    SerialBT.print(", ");
    SerialBT.print(gy, 5);
    SerialBT.print(", ");
    SerialBT.print(gz, 5);
    SerialBT.print(", ");
    SerialBT.print(pitch, 5);
    SerialBT.print(", ");
    SerialBT.print(roll, 5);
    SerialBT.print(", ");
    SerialBT.print(yaw, 5);
    SerialBT.print(", ");
    SerialBT.print(ax, 5);
    SerialBT.print(", ");
    SerialBT.print(ay, 5);
    SerialBT.print(", ");
    SerialBT.println(az, 5);
    lastPrintMillis = currentMillis;
  }
}

void reInitHandler() {
  gx = 0.0;
  gy = 0.0;
  gz = 0.0;
  SerialBT.println("Initialize");
}

void testHandler() {
  Serial.println("Hello");
}