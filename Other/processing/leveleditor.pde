import java.util.Date;

private color BLACK = color(0);
private color WHITE = color(255);
private color RED = color(255,0,0);
private color GREEN = color(0,255,0);
private color BLUE = color(0,0,255);

int radius = 1000;
int divisionDistance = 100;
int cakeSlices = 12;

// zooming and navigating
int xShift = 0;
int yShift = 0;
float scaleFactor = 1;

int circles;
float angleIncrement;
color[][] fillData;

color currentColor = WHITE;

void setup() {
  size(1000, 1000, P2D);
  noStroke();
  ellipseMode(CENTER);
  init();
  prepareFillData();
}

void prepareFillData() {
  for (int i = 0; i < circles; i++) {
    for (int j = 0; j < cakeSlices; j++) {
      fillData[i][j] = BLACK;
    }
  }
}

void init() {
  circles = (radius/divisionDistance);
  angleIncrement = 360.0/cakeSlices;
  if (fillData != null) {
    color[][] temp = new color[circles][cakeSlices];
    
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

void draw() { //<>//
  background(BLACK);
  translate(xShift, yShift);
  scale(scaleFactor);
  
  for (int circle = circles-1; circle >= 0; circle--) {
    int circleRadius = (circle+1)*divisionDistance;
    
    fill(0);
    ellipse(width/2, height/2, circleRadius, circleRadius);
    
    for (int i = 0; i < cakeSlices; i++) {
      if (fillData[circle][i] != BLACK) {
        fill(fillData[circle][i]);
        float fromRad = radians(angleIncrement*i);
        float toRad = radians(angleIncrement*(i+1));
        arc(width/2, height/2, circleRadius, circleRadius, fromRad, toRad);
      } 
    }
  }
}
int prevCircle = -1, prevCake = -1;
int circleIdx, cakeIdx;

void calculateCircleAndCakeIdx() {
  float shiftedX = (mouseX-xShift)/scaleFactor; 
  float shiftedY = (mouseY-yShift)/scaleFactor;
  float distance = dist(width/2, height/2, shiftedX, shiftedY);
  circleIdx = (int)distance*2/divisionDistance;
  if (circleIdx >= circles)
    circleIdx = circles - 1;
  
  PVector v1 = new PVector(width/2, height/2);
  PVector v2 = new PVector(shiftedX, shiftedY);
  PVector minus = v2.sub(v1);
  float a = minus.heading();
  float degrees = degrees(a) < 0 ? 360 + degrees(a) : degrees(a);
  cakeIdx = (int)(degrees/angleIncrement);
  if (cakeIdx >= cakeSlices)
    cakeIdx = cakeSlices - 1;
}

void mouseDragged() {
  
  if (mouseButton == RIGHT) {
    xShift = xShift + (mouseX - pmouseX);
    yShift = yShift + (mouseY - pmouseY);
  } else {
    calculateCircleAndCakeIdx();
    
    if (prevCircle != circleIdx || prevCake != cakeIdx) {
      fillDataWithOpposite();
      prevCircle = circleIdx;
      prevCake = cakeIdx;
    }
  }
}

void mousePressed() {
  if (mouseButton == LEFT) {
    calculateCircleAndCakeIdx();
    fillDataWithOpposite();
  }
}

void mouseWheel(MouseEvent e)
{
  scaleFactor += e.getAmount() / 100.0;
 
  if (scaleFactor < 1.0) scaleFactor = 1.0;
  if (scaleFactor > 3.0) scaleFactor = 3.0;
}

private void fillDataWithOpposite() {
  if (fillData[circleIdx][cakeIdx] != BLACK) {
    fillData[circleIdx][cakeIdx] = BLACK;
  } else {
    fillData[circleIdx][cakeIdx] = currentColor;
  }
}

void keyPressed() {
  println("key pressed: " + key);
  switch(key) {
    case 's':
      Date date = new Date();
      save("station_" + 
        date.getHours() + "_" + 
        date.getMinutes() + "_" + 
        date.getSeconds() + ".png");
      break;
    case 'e':
      divisionDistance += 5;
      break;
    case 'd':
      if (divisionDistance > 5)
        divisionDistance -= 5;
      break;
    case 'r':
      cakeSlices += 1;
      break;
    case 'f':
      if (cakeSlices > 1)
        cakeSlices -= 1;
      break;
    case '0':
      currentColor = WHITE;
      break;
    case '1':
      currentColor = RED;
      break;
    case '2':
      currentColor = GREEN;
      break;
    case '3':
      currentColor = BLUE;
      break;
    case 'a':
      scaleFactor = 1;
      xShift = 0;
      yShift = 0;
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