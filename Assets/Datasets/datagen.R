library(random)

closeRadius <- 0.5
minRadius <- 0.05
maxRadius <- 1

#angles <- sample(1:360, 100)

fluctuate_values <- function(base_value, fluc) 
{
  value <- -1
  while(value < 0 || value > 1 || value == base_value)
  {
    variance <- runif(1, -fluc, fluc)
    value <- base_value + variance
  }
  return(round(base_value + variance, 2))
}

# Task 1
# 3 (vis) x 2 (distances) x 4 repetitions = 24 questions

generate_task1_dataset <- function()
{
  # Generate 4 close radius, 4 far radius, 
  # Each of 4 close radius will be used to generate 3 datapoints 
  # with random angles (each datapoints for each vis)
  # For the rest of  76 (100-24), would be randomly placed in uniform distribution
  
  # repetition
  task1_repetition <- 4
  vis_levels <- 3
  radius_level <- 2
  datasets_count <- vis_levels * radius_level * task1_repetition
  angles <- randomNumbers(n=datasets_count, min=1, max=360, col=1)
  
  
  sCloseRadii <- runif(task1_repetition, minRadius, closeRadius)
  sFarRadii <- runif(task1_repetition, closeRadius, maxRadius)
  
  dataPoints <- data.frame("id"=integer(), "x"=double(), "y"=double(), "value"=double(), stringsAsFactors = F)
  names(dataPoints)
  currentAngleIdx <- 1
  # Close point
  recur <- 0
  for (radius in sCloseRadii)
  {
    base_value <- round(runif(1, 0, 1), 2)
    # 3 trials w/ close radius
    for (idx in 1:3) 
    {
      value <- fluctuate_values(base_value, 0.05)
      angle <- angles[currentAngleIdx]
      currentAngleIdx <- currentAngleIdx + 1
      x <- sin(angle * pi/180) * radius
      y <- cos(angle * pi/180) * radius
      id <- idx*8-7 + recur
      dataPoints[id, ] <- c(id, x, y, value)
    }
    recur <- recur + 1
  }
  
  recur <- 0
  for (radius in sFarRadii)
  {
    base_value <- round(runif(1, 0, 1), 2)
    # 3 trials w/ far radius
    for (idx in 1:3) 
    {
      value <- fluctuate_values(base_value, 0.05)
      angle <- angles[currentAngleIdx]
      currentAngleIdx <- currentAngleIdx + 1
      x <- sin(angle * pi/180) * radius
      y <- cos(angle * pi/180) * radius
      id <- idx*8-7 + recur + task1_repetition
      dataPoints[id, ] <- c(id, x, y, value)
    }
    recur <- recur + 1
  }
  
  # Fill out the rest of 76 slots
  #remaining_slots = n - (vis_levels * radius_level * task1_repetition)
  #closeRadii <- runif(remaining_slots/2, minRadius, closeRadius)
  #farRadii <- runif(remaining_slots/2, closeRadius, maxRadius)
  #allRadius <- c(closeRadii, farRadii)
  #remaining_values <- round(runif(remaining_slots, 0, 1), 2)
  #dataPoints[25:100, 4] <- remaining_values
  
  #for (idx in 25:100)
  #{
  #  radius <- allRadius[idx-24]
  #  angle <- angles[currentAngleIdx]
  #  currentAngleIdx <- currentAngleIdx + 1
  #  x <- sin(angle * pi/180) * radius
  #  y <- cos(angle * pi/180) * radius
  #  dataPoints[idx, 1:3] <- c(idx, x, y)
  #}
  
  # Write to CSV file
  write.csv(dataPoints, file="dataset_est.csv", quote=F, row.names=F)
}
generate_task1_dataset()
# Task 2
# Which is higher
# Each vis: 1-18-37
generate_task2_dataset <- function()
{
  task2_repetition <- 3
  vis_levels <- 3
  radius_levels <- 3
  fov_levels <- 2
  datasets_count <- task2_repetition * vis_levels * radius_levels * fov_levels * 2
  
  questions <- data.frame("id"=integer(), 
                          "vis"=character(), 
                          "radius1"=double(), "radius2"=double(),
                          "fov"=integer(), 
                          "angle1"=integer(), "angle2"=integer(),
                          "re"=integer(), stringsAsFactors = F,
                          "answer"=integer())
  
  for (id in 1:(datasets_count/2))
  {
    questions[id, 1] <- id
  }
  
  questions[1:(datasets_count/2/3), 2] <- "v1"
  questions[((datasets_count/2/3)+1):(datasets_count/2/3*2), 2] <- "v2"
  questions[(datasets_count/2/3*2+1):(datasets_count/2), 2] <- "v3"
  
  # Generate datasets_count number of close radius
  sCloseRadii <- runif(datasets_count/2, min=minRadius, max=closeRadius)
  closeRadiusIdx <- 1
  # Generate datasets_count number of far radius
  sFarRadii <- runif(datasets_count/2, min=closeRadius, max=maxRadius)
  farRadiusIdx <- 1
  
  # FOV
  max_in_fov <- 110
  min_in_fov <- 5
  max_out_fov <- 179
  
  # Loop for in_fov
  # n = c/2/3/2 (number of questions is half of datapoints, 3 vis share the same fov, 2 levels of fov)
  in_fovs <- randomNumbers(n=datasets_count/2/3/2, min=min_in_fov, max=max_in_fov, col=1)
  currentInFovIdx <- 1
  # Loop for out_fov
  out_fovs <- randomNumbers(n=datasets_count/2/3/2, min=max_in_fov, max=max_out_fov, col=1)
  currentOutFovIdx <- 1
  
  for (rep in 0:(task2_repetition-1))
  {
    # CC
    for (r in 0:1)
    {
      radius1 <- sCloseRadii[closeRadiusIdx]
      closeRadiusIdx <- closeRadiusIdx + 1
      radius2 <- sCloseRadii[closeRadiusIdx]
      closeRadiusIdx <- closeRadiusIdx + 1
      for (id in 1:3)
      {
        questions[id*6*task2_repetition-(6 * task2_repetition - 1)+r*task2_repetition+rep, 3:4] <- c(radius1, radius2)
      }
    }
    
    # CF
    for (r in 0:1)
    {
      radius1 <- sCloseRadii[closeRadiusIdx]
      closeRadiusIdx <- closeRadiusIdx + 1
      radius2 <- sFarRadii[farRadiusIdx]
      farRadiusIdx <- farRadiusIdx + 1  
      
      for (id in 1:3)
      {
        questions[id*6*task2_repetition-(6 * task2_repetition - 1)+(task2_repetition*2)+r*task2_repetition+rep, 3:4] <- c(radius1, radius2)
      }
    }
    
    # FF
    for (r in 0:1)
    {
      radius1 <- sFarRadii[farRadiusIdx]
      farRadiusIdx <- farRadiusIdx + 1
      radius2 <- sFarRadii[farRadiusIdx]
      farRadiusIdx <- farRadiusIdx + 1
      
      for (id in 1:3)
      {
        questions[id*6*task2_repetition-(6 * task2_repetition - 1)+(task2_repetition*2*2)+r*task2_repetition+rep, 3:4] <- c(radius1, radius2)
      }
    }
    
    # Loop in fov
    for (r in 0:2)
    {
      fov <- in_fovs[currentInFovIdx]
      currentInFovIdx <- currentInFovIdx + 1
      for (id in 1:3)
      {
        idx <- id*6*task2_repetition-(6 * task2_repetition - 1)+r*2*task2_repetition+rep
        questions[idx, 5] <- fov
        questions[idx, 8] <- rep+1
      }
    }
    
    for (r in 0:2)
    {
      fov <- out_fovs[currentOutFovIdx]
      currentOutFovIdx <- currentOutFovIdx + 1
      for (id in 1:3)
      {
        idx <- id*6*task2_repetition-(6 * task2_repetition - 1)+r*2*task2_repetition+task2_repetition+rep
        questions[idx, 5] <- fov
        questions[idx, 8] <- rep+1
      }
    }
  }
  
  dataPoints <- data.frame("id"=integer(), "x"=double(), "y"=double(), "value"=double(), stringsAsFactors = F)
  angles1 <- randomNumbers(n=datasets_count/2, min=1, max=360, col=1)
  currentAngleIdx <- 1
  # Angles1
  for (id in 1:(datasets_count/2))
  {
    radius1 <- questions[id,"radius1"]
    radius2 <- questions[id,"radius2"]
    fov <- questions[id, "fov"]
    angle1 <- angles1[currentAngleIdx]
    currentAngleIdx <- currentAngleIdx + 1
    
    x1 <- sin(angle1 * pi/180) * radius1
    y1 <- cos(angle1 * pi/180) * radius1
    
    angle2 <- angle1 - fov
    x2 <- sin(angle2 * pi/180) * radius2
    y2 <- cos(angle2 * pi/180) * radius2
    
    questions[id, 6:7] <- c(angle1, angle2)
    
    if ((id %% 2) == 1)
    {
      questions[id, 9] <- 1
    } else
    { 
      questions[id, 9] <- 2
    }
  }
}

