import java.util.Date;

int radius = 1000;
int divisionDistance = 100;
int cakeSlices = 12;

int pixelCorrection = 0;
int circles;
float angleIncrement;
int[][] fillData;

void setup() {
  size(1000, 1000);
  smooth();
  noStroke();
  ellipseMode(CENTER);
  init();
  prepareFillData();
}

void init() {
  circles = (radius/divisionDistance);
  angleIncrement = 360.0/cakeSlices;
  if (fillData != null) {
    int[][] temp = new int[circles][cakeSlices];
    
    for (int i = 0; i < fillData.length; i++) {
      for (int j = 0; j < fillData[i].length; j++) {
        try {
          temp[i][j] = fillData[i][j];
        } catch (Exception e) {
          // who cares? :D 
        }
      }
    }
    fillData = temp;
  } else {
    fillData = new int[circles][cakeSlices];
  }
}

void draw() {
  background(0);
  for (int circle = circles-1; circle >= 0; circle--) {
    int circleRadius = (circle+1)*divisionDistance;
    
    fill(0);
    ellipse(width/2, height/2, circleRadius-pixelCorrection, circleRadius-pixelCorrection);
    fill(255);
    
    for (int i = 0; i < cakeSlices; i++) {
      if (fillData[circle][i] == 1) {
        float fromRad = radians(angleIncrement*i);
        float toRad = radians(angleIncrement*(i+1) + pixelCorrection);
        arc(width/2, height/2, circleRadius, circleRadius, fromRad, toRad);
      } 
    }
  } //<>//
}
int prevCircle = -1, prevCake = -1;
int circleIdx, cakeIdx;

void calculateCircleAndCakeIdx() {
  float distance = dist(width/2, height/2, mouseX, mouseY);
  circleIdx = (int)distance*2/divisionDistance;
  if (circleIdx >= circles)
    circleIdx = circles - 1;
  
  PVector v1 = new PVector(width/2, height/2);
  PVector v2 = new PVector(mouseX, mouseY);
  PVector minus = v2.sub(v1);
  float a = minus.heading();
  float degrees = degrees(a) < 0 ? 360 + degrees(a) : degrees(a);
  cakeIdx = (int)(degrees/angleIncrement);
  if (cakeIdx >= cakeSlices)
    cakeIdx = cakeSlices - 1;
}

void mouseDragged() {
  calculateCircleAndCakeIdx();
  
  if (prevCircle != circleIdx || prevCake != cakeIdx) {
    fillData[circleIdx][cakeIdx] = abs(1 - fillData[circleIdx][cakeIdx]);
    prevCircle = circleIdx;
    prevCake = cakeIdx;
  }
}

void mousePressed() {
  calculateCircleAndCakeIdx();
  fillData[circleIdx][cakeIdx] = abs(1 - fillData[circleIdx][cakeIdx]);
}

void keyPressed() {
  Date date = new Date();
  println("key pressed: " + key);
  switch(key) {
    case 's':
      save("station_" + 
        date.getHours() + "_" + 
        date.getMinutes() + "_" + 
        date.getSeconds() + ".png");
      break;
    case 'e':
      divisionDistance += 10;
      break;
    case 'd':
      divisionDistance -= 10;
      break;
    case 'r':
      cakeSlices += 1;
      break;
    case 'f':
      cakeSlices -= 1;
      break;
  }
  init();
}

void lineAngle(int x, int y, float angle, float length)
{
  angle = radians(angle);
  float toX = x+cos(angle)*length;
  float toY = y-sin(angle)*length;
  line(x, y, toX, toY);
}

void prepareFillData() {
  for (int i = 0; i < circles; i++) {
    for (int j = 0; j < cakeSlices; j++) {
      fillData[i][j] = 0;
    }
  }
}