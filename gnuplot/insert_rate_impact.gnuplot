set term pdfcairo size 8in,6in

set xlabel "insert rate"
set ylabel "edit rate"

unset key
set tic scale 0

# Color runs from white to green
# set palette rgbformula -7,2,-7
set cbrange [0:5]
set cblabel "Score"

# set logscale cb
# unset cbtics

set xtics offset 3
set xtics 0.02,0.06,0.3

set ytics offset 0,1
set ytics 0.0,0.2,0.8

set xrange [0:0.3]
set yrange [0:1]

set view map
set pm3d interpolate 1,1

set output "insert_rate_impact.pdf"

splot "insert_rate_impact.dat" u 2:1:3 with pm3d

set term png size 1280,960

set output "insert_rate_impact.png"

splot "insert_rate_impact.dat" u 2:1:3 with pm3d