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
  task2_repetition <- 1
  vis_levels <- 3
  radius_levels <- 3
  fov_levels <- 2
  datasets_count <- task2_repetition * vis_levels * radius_levels * fov_levels
  angles1 <- randomNumbers(n=datasets_count, min=1, max=360, col=1)
  currentAngleIdx <- 1
  
  max_in_fov <- 110
  min_in_fov <- 5
  max_out_fov <- 179

  # Generate datasets_count number of close radius
  closeRadius <- runif(datasets_count, min=minRadius, max=closeRadius)
  closeRadiusIdx <- 1
  # Generate datasets_count number of far radius
  farRadius <- runif(datasets_count, min=closeRadius, max=maxRadius)
  farRadiusIdx <- 1
  
  dataPoints <- data.frame("id"=integer(), "x"=double(), "y"=double(), "value"=double(), stringsAsFactors = F)
  # Loop for in_fov
  in_fovs <- randomNumbers(n=datasets_count/2, min=min_in_fov, max=max_in_fov, col=1)
  for (fov in in_fovs)
  {
    # Each vis: same fov, same radii, different angles
    # Radius levels: C-C C-F F-F
    
    # C-C
    cc_startIdx = 1
    #for (r in 1:datasets_count/3)
    #{
      radius1 <- closeRadius[closeRadiusIdx]
      closeRadiusIdx <- closeRadiusIdx + 1
      radius2 <- closeRadius[closeRadiusIdx]
      closeRadiusIdx <- closeRadiusIdx + 1
      
      for (idx in 1:3)
      { 
        angle1 = angles1[currentAngleIdx]
        currentAngleIdx <- currentAngleIdx + 1
        
        angle2 <- angle1 - fov
        
        x1 <- sin(angle1 * pi/180) * radius1
        y1 <- cos(angle1 * pi/180) * radius1
        id1 <- idx * 18 - 17
        dataPoints[id1, ] <- c(id1, x1, y1, 0)
        
        x2 <- sin(angle2 * pi/180) * radius2
        y2 <- cos(angle2 * pi/180) * radius2
        id2 <- id1 + 1
        dataPoints[id2, ] <- c(id2, x2, y2, 0)
      #}
    }
  }
  
  # Loop for out_fov
  out_fovs <- randomNumbers(n=datasets_count/2, min=max_in_fov, max=max_out_fov, col=1)
  
  # Create pairs of radius
  
  # Each of pair will be used for 3 types of vis
  # For each type of vis, randomise the angle for
  
  # In fov
  # 
  
}