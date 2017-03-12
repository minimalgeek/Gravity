public class Tile {
  public int thickness;
  public int outerRadius;
  public int divisions;

  public float angle;
  public int thicknessPX;
  public int outerRadiusPX;

  public Tile() {
  }

  public Tile(String line, int cmToPixel) throws Exception {
    String[] lineParts = line.split("\t");
    if (lineParts.length != 3) {
      throw new Exception("Line is not valid: " + line);
    }

    this.thickness = Integer.parseInt(lineParts[0]);
    this.outerRadius = Integer.parseInt(lineParts[1]);
    this.divisions = Integer.parseInt(lineParts[2]);

    this.angle = 360.0/this.divisions;
    this.thicknessPX = this.thickness * cmToPixel;
    this.outerRadiusPX = this.outerRadius * cmToPixel;
  }
}