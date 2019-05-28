library(ggplot2)
library(dplyr)
task2_results <- read.csv("../Results/0__PickLargerDataPoint.csv")

map_cone_task2 <- task2_results[task2_results[5] == 'MapCone',]
