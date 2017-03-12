PGraphics main, mask;
PImage img;

String prefix = null;
int pixelPerCm = 1;
ArrayList<Tile> tiles = new ArrayList<Tile>();

color transparent = color(0, 0);

void setup() {

  String lines[] = loadStrings("block25.txt");
  println("there are " + lines.length + " lines");

  prefix = lines[0];
  pixelPerCm = Integer.parseInt(lines[1]);

  for (int i = 2; i < lines.length; i++) {
    try {
      tiles.add(new Tile(lines[i], pixelPerCm));
    } 
    catch (Exception e) {
      println("Error: " + e.getMessage());
    }
  }

  for (Tile tile : tiles) {
    drawAndSaveTransparentImage(tile);
  }
  
  exit();
}

void drawAndSaveTransparentImage(Tile tile) {
  float fromAngleRad = radians(90-tile.angle/2);
  float toAngleRad = radians(90+tile.angle/2);
  int coverArcSize = tile.outerRadiusPX - tile.thicknessPX;
  int distanceHalf = tile.outerRadiusPX/2;

  main = createArc(distanceHalf, tile.outerRadiusPX, fromAngleRad, toAngleRad);
  mask = createArc(distanceHalf, coverArcSize, fromAngleRad-0.1, toAngleRad+0.1);

  maskAndSave(tile);
}

PGraphics createArc(int center, int size, float fromAngle, float toAngle) {
  PGraphics pg = createGraphics(center*2, center, JAVA2D);
  pg.noSmooth();
  pg.beginDraw();
  pg.noStroke();
  pg.translate(0, -center);
  pg.fill(255, 255);
  pg.arc(center, center, size, size, fromAngle, toAngle);
  pg.endDraw();
  return pg;
}

void maskAndSave(Tile tile) {
  main.loadPixels();
  mask.loadPixels();
  for (int i=0; i<mask.pixels.length; i++) {
    int a = (mask.pixels[i] >> 24) & 0xFF;
    if (a > 0) {
      main.pixels[i] = transparent;
    }
  }
  main.updatePixels();
  main.save(prefix + "_" + tile.thickness + "_" + tile.outerRadius + "_" + tile.divisions + ".png");
}