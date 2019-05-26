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
