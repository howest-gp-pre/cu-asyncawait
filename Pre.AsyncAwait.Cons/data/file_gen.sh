#!/bin/bash 
for i in {1..1000}; do
  echo $(( $RANDOM % 50 + 1 )) >./calculations/data$i.txt
done
