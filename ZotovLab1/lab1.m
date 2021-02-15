% part 4 - my signal
T = 1/10000;  % 0.1 мс
t = 0:T:100*T;
f = sin(2*pi*500*t) + sin(2*pi*1000*t)/2;
plot(t,f);

Ck = zeros(size(t));
N = length(t);
% реализуем ДПФ "самостоятельно" ;)
for k = 1: N
  for i = 1: N
    Ck(k) = Ck(k) + f(i)*exp(-j*2*pi*k*i/N);
  end
  Ck(k) = Ck(k) / N;
end

Ck = fftshift(Ck)

f = [-(N-1)/2:(N-1)/2] ./ max(t);
%f = [0:N-1] ./ max(t);
plot(f, abs(Ck));
return

% program is improved 14.02.2011 by L.V. Zotov
clear;


%--------- Part 1---------------------
% fft of artificial signal

N_signal=1024;
% generating two-sin signal
%for (k=1:1:N_signal)
%    signal(k)=sin(2*pi/10*(k-1))+sin(2*pi/100*(k-1));
%end;

k=1:1:N_signal;
signal=sin(2*pi/10*(k-1))+sin(2*pi/100*(k-1));

% try to add noise with Coef*randn([1,N_signal])

plot(signal);

%fast Fourier transform calculation

Ftrns_signal=fft(signal);

%amplitude spectrum calculation
apl_spectrum=abs(Ftrns_signal);

plot(apl_spectrum);

return
%----------- Part 2-----------------------------------------
% download file http://hpiers.obspm.fr/iers/eop/eopc01/eopc01.1900-now.dat
% to the folder (change if needed):
% cd C:\work\course\filtr\eng\Lab1;

fin=fopen('eopc01.1900-now.dat','rt');
fgetl(fin);
A=fscanf(fin,'%f',[11 inf]);% A - array of data
fclose(fin);

%determining the size of the signal
l=size(A);
N=l(2);

%selecting the rows of the Array
Date=A(1,1:N);
X_pole=A(2,1:N);
Y_pole=A(4,1:N);
dt=Date(2)-Date(1);

plot(Date(2:N)-Date(1:N-1))

plot3(X_pole,Y_pole,Date)

plot(X_pole);

%selecting the length of the part of the signal will be trasformed
N_ft=N;
%fast Fourier transform calculation
Ftrns_X=fft(X_pole,N_ft);


% frequency calculation
% for the half of the spectrum not counting 1 coefficient - an avarage
% N_ft is odd or even - ? 
N_half=round((N_ft-1)/2);
freq=(1:N_half)/N_ft/dt;

%amplitude spectrum calculation

ampl_spectrum_X=abs(Ftrns_X)/N_ft;

% we plot spectrum multiplied by two to take into account energy in the
% second part
plot(freq, 2*ampl_spectrum_X(2:N_half+1));
title('amplitude spectrum - module of Fourier-transformation ')
xlabel('frequency')

% periods=1./freq;
% plot(periods, 2*apl_spectrum_X(2:N_half+1));
% title('periodogramm')
% xlabel('frequency')
% 


%------------ now let's calculate the complex spectrum
%functions path
 %  slow Fourier transform    
addpath( './functions')
XY=X_pole-i*Y_pole;
[ spectrXY, freqXY] = spect_fftn(Date,XY);

plot(freqXY', abs(spectrXY)')   
title('amplitude spectrum - module of Fourier-transformation ')
xlabel('frequency, cycles per year')



%Co L.V. Zotov