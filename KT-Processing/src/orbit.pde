void setup()
{
  background(#ECECEC);
  stroke(0);
  smooth();
  strokeWeight(1.0);
  
  size(400,400);
  frameRate(62.5);
  rect(0,0,399,399);
  noLoop();
}



float initialX;
float initialY;

float fakeMouseX;
float fakeMouseY;


void mousePressed()
{
  initialX = mouseX;
  initialY = mouseY;
  
  fakeMouseX = mouseX;
  fakeMouseY = mouseY;
  
  loop();
  noCursor();
}

float radius(float x, float y)
{
  return sqrt(x * x + y * y);
}

float angle(float x, float y)
{
  return atan2(y, x);
}

void draw()
{
  float realDeltaX = mouseX - fakeMouseX;
  float realDeltaY = mouseY - fakeMouseY;
  
  float shiftX = initialX - 200;
  float shiftY = initialY - 200;
  
  float origR = radius(shiftX, shiftY);
  float origTheta = angle(shiftX, shiftY);
  
  float newR = radius(shiftX + realDeltaX, shiftY + realDeltaY);
  float newTheta = angle(shiftX + realDeltaX, shiftY + realDeltaY);
  
  // modify the following two differences to manipulate compass strength
  float finalR = (newR - origR) + origR;
  float finalTheta = (newTheta - origTheta) + origTheta + 0.1;
  
  float deltaX = finalR * cos(finalTheta) - shiftX;
  float deltaY = finalR * sin(finalTheta) - shiftY;
  
  line(initialX, initialY, initialX + deltaX, initialY + deltaY);
    
  initialX = initialX + deltaX;
  initialY = initialY + deltaY;
    
  fakeMouseX = mouseX;
  fakeMouseY = mouseY;
}

void mouseReleased()
{
  noLoop();
  cursor(ARROW);
}


