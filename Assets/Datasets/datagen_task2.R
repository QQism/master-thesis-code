library(random)

closeRadius <- 0.5
minRadius <- 0.05
maxRadius <- 1

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
                          "answer"=integer(), 
                          "base_value"=double(),
                          "diff_value"=double(),
                          "value1"=double(),
                          "value2"=double())
  
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
  
  # Base values
  base_values <- randomNumbers(n=datasets_count/2/3, min=0, max = 100, col=1)
  base_values <- base_values / 100 # normalise [0, 100] to [0, 1]
  currentBaseValues <- 1
  
  # Diff values: min 5%, max 10%
  diff_values <- randomNumbers(n=datasets_count/2/3, min=5, max = 10, col=1)
  diff_values <- diff_values / 100
  currentDiffValues <- 1
  
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
    
    # Base value
    #rep <- 0
    #for (r in 1:(datasets_count/2/3))
    for (r in 1:6)
    {
      base_value <- base_values[currentBaseValues]
      currentBaseValues <- currentBaseValues + 1
      
      diff_value <- diff_values[currentDiffValues]
      currentDiffValues <- currentDiffValues + 1
      
      for (id in 1:3)
      {
        idx <- id*6*task2_repetition-(6 * task2_repetition - 1)+r*task2_repetition-task2_repetition+rep
        questions[idx, 10:11] <- c(base_value, diff_value)
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
    angle2 <- angle1 - fov

    questions[id, 6:7] <- c(angle1, angle2)
    
    base_value = questions[id, 10]
    diff_value = questions[id, 11]
    
    value1 <- 0
    value2 <- 0
    
    adjustment = "increase"
    if ((base_value+diff_value) >= 1)
    {
      adjustment = "decrease"
    }
    
    if ((id %% 2) == 1)
    {
      questions[id, 9] <- 1
      
      if (adjustment ==  "increase")
      { 
        value1 <- base_value + diff_value
        value2 <- base_value
      } else
      { 
        value1 <- base_value
        value2 <- base_value - diff_value
      }
    } else
    { 
      questions[id, 9] <- 2
      
      if (adjustment ==  "increase")
      { 
        value1 <- base_value
        value2 <- base_value + diff_value
      } else
      { 
        value1 <- base_value - diff_value
        value2 <- base_value
      }
    }
    
    questions[id, 12] <- value1
    questions[id, 13] <- value2
    
    x1 <- sin(angle1 * pi/180) * radius1
    y1 <- cos(angle1 * pi/180) * radius1
    
    x2 <- sin(angle2 * pi/180) * radius2
    y2 <- cos(angle2 * pi/180) * radius2
    
    id1 <- id * 2 - 1
    id2 <- id1 + 1
    dataPoints[id1, ] <- c(id1, x1, y1, value1)
    dataPoints[id2, ] <- c(id2, x2, y2, value2)
  }
  
  return(dataPoints)
}

dataPoints1 <- generate_task2_dataset()

write.csv(dataPoints1, file="dataset_higher1.csv", quote=F, row.names=F)
  
  