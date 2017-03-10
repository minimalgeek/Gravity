PGraphics main, mask;
PImage img;

int fromRadius = 1000;
int toRadius = 2000;
int radiusIncrement = 500;
int thickness = 100;

float fromAngle = 40f;
float toAngle = 90f;
float angleIncrement = 20f;

color transparent = color(0, 0);

void setup() {
  for (int distance = fromRadius; distance <= toRadius; distance+=radiusIncrement) {
    for (float angle = fromAngle; angle <= toAngle; angle += angleIncrement) {
      drawAndSaveTransparentImage(distance, angle);
    }
  }
  exit();
}

void drawAndSaveTransparentImage(int distance, float angle) {
  float fromAngleRad = radians(90-angle/2);
  float toAngleRad = radians(90+angle/2);
  int coverArcSize = distance-thickness;
  int distanceHalf = distance/2;

  main = createArc(distanceHalf, distance, fromAngleRad, toAngleRad);
  mask = createArc(distanceHalf, coverArcSize, fromAngleRad-0.1, toAngleRad+0.1);

  maskAndSave(distance, angle);
}

PGraphics createArc(int center, int size, float fromAngle, float toAngle) {
  PGraphics pg = createGraphics(center*4, center*2, JAVA2D);
  pg.noSmooth();
  pg.beginDraw();
  pg.noStroke();
  pg.scale(2);
  pg.translate(0, -center*1.25);
  pg.fill(255, 255);
  pg.arc(center, center, size, size, fromAngle, toAngle);
  pg.endDraw();
  return pg;
}

void maskAndSave(int distance, float angle) {
  main.loadPixels();
  mask.loadPixels();
  // go over all the pixels in the mask
  for (int i=0; i<mask.pixels.length; i++) {
    // get transparency of mask pixel
    int a = (mask.pixels[i] >> 24) & 0xFF;
    // if it is not transparenct (aka something is drawn on the mask)
    if (a > 0) {
      // make the main pixel in this location transparent
      main.pixels[i] = transparent;
    }
  }
  main.updatePixels();
  main.save("transparent_" + distance + "_" + angle + ".png");
}