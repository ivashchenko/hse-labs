[X,Y] = meshgrid(-1:0.5:30,-1:0.5:30);
Z = 0.2*0.5*exp(-0.2*X-0.5*Y)
%surf(X,Y,cumsum(cumsum(Z,1),2))

surf(X,Y,cumsum(Z,1))