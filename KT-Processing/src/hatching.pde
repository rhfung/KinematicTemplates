void setup()
{
  background(#ECECEC);
  stroke(0);
  smooth();
  strokeWeight(1.0);
  
  size(400,400);
  frameRate(30);
  rect(0,0,399,399);
  noLoop();
}

void draw()
{

}

float initialX;
float initialY;
float deltaX = 0;
float deltaY = 0;

float fakeMouseX;
float fakeMouseY;
float realDeltaX;
float realDeltaY;

void mousePressed()
{
  loop();
  initialX = mouseX;
  initialY = mouseY;
  
  fakeMouseX = mouseX;
  fakeMouseY = mouseY;
}

void mouseDragged()
{
  realDeltaX = mouseX - fakeMouseX;
  realDeltaY = mouseY - fakeMouseY;
  
  // modify the following two delta values to adjust
  // attenutation on the horizontal or vertical axis
  deltaX =  realDeltaX;
  deltaY =  realDeltaY * 0.25;
  
  line(initialX, initialY, initialX + deltaX, initialY + deltaY);
    
  initialX = initialX + deltaX;
  initialY = initialY + deltaY;
    
  fakeMouseX = mouseX;
  fakeMouseY = mouseY;
}

void mouseReleased()
{
  noLoop();
}


