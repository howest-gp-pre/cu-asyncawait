for i in {1..100}; do
  echo $(( $RANDOM % 50 + 1 )) >./data$i.txt
done
