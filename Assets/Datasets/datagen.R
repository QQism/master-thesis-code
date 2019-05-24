library(random)

n <- 100
closePoints <- farPoints <- n/2

closeRadius <- 0.5
minRadius <- 0.027
maxRadius <- 1

angles <- runif(100, 1, 360)
closeRadii <- runif(50, minRadius, closeRadius)
farRadii <- runif(50, closeRadius, maxRadius)

# Task 1
# 3 (vis) x 2 (distances) x 4 repetitions = 24 questions

# Generate 4 close radius, 4 far radius, 
# random the angle
# Each of 4 close radius will be used to generate 3 datapoints 
# with random angles (each datapoints for each vis)
# For the rest of  76 (100-24), would be randomly placed in uniform distribution


# Close point
for (i in 1:closePoints) 
{
}
