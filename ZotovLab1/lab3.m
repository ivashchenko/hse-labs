% program is written 20.01.2009 by L.V. Zotov
clear;

N_signal=1024;
% generating two-sin signal
for (k=1:1:N_signal)
    garm(k)=0.05*sin(2*pi/10*(k-1))+0.05*sin(2*pi/100*(k-1));
end;

plot(garm);

% ARMA process generating
ar(1)=0.1*randn(1);
ar(2)=-0.2*ar(1)+0.1*randn(1);
for (i=3:1:N_signal)
    ar(i)=0.7*ar(i-1)-0.2*ar(i-2)+0.1*randn(1);
end;

plot(ar, ':');
legend({'Сгенерированный шум'},'Location','southwest')

%making a sum
signal=garm+ar;%garm+
plot(signal);
legend({'Исходный сигнал + шум'},'Location','southwest')
%--------------------------------------------------------------------------
%-------------

%output to the file
%chnage the path
%cd 'd:/Р¤РёР»СЊС‚СЂР°С†РёСЏ';
fout=fopen('signal.dat','wt');
for(i=1:1:N_signal)
 fprintf(fout,'%12.6f%12.6f\n',i,signal(i)); 
end;
fclose(fout);

savefile = 'signal.mat';
save(savefile,'signal');

% fast Fourier transformation
N_ft=N_signal;

Ftrns_signal=fft(signal,N_ft);

% frequency calculation
% for the half of the spectrum not counting 1 coefficient - an avarage
% N_ft is odd or even - ? 
dt=1;
N_half=round((N_ft-1)/2);
freq=(1:N_half)/N_ft/dt;

ampl_spectrum=abs(Ftrns_signal)/N_ft;

energy_spectrum=Ftrns_signal.*conj(Ftrns_signal)/N_ft^2;

% plot half of the spectrum without multiplication by 2
subplot(2,1,1); 
plot(freq, ampl_spectrum(2:N_half+1));
title('amplitude spectrum ')
xlabel('frequency')

subplot(2,1,2); 
plot(freq, energy_spectrum(2:N_half+1));
title('energy spectrum ')
xlabel('frequency')

clf;

%--------------------------------------------------------------------------------------------------
% avarage substraction
abs(Ftrns_signal(1))/N_ft %equal
mean(signal)
signal_centered=signal-mean(signal);

% ACF calculation

for(tau=1:1:N_signal)
 acf(tau)=0;
 for(j=1:1:N_signal-tau)
    acf(tau)=acf(tau)+signal_centered(j)*signal_centered(j+tau-1);
 end;
  acf(tau)= acf(tau)/(N_signal-tau+1);
end;
plot(acf);

clf;
tiledlayout(2,1)
nexttile;
plot(acf(1:254));
nexttile;

F = abs(fft([signal zeros(size(signal))]))
S = F.*F
acf2 = fftshift(ifft(S))/N_signal

acf3 = xcorr(signal)/N_signal
idx = find(acf2 == max(acf2))
acf2 = acf2(idx: idx+254)

plot(acf2(1:254)-acf3(1:254));

%--------------------------------------------------------------------------------------------------

% Power Spectrum Dencity calculations

Ftrns_acf=fft(acf,N_ft);

plot(abs(Ftrns_acf)/N_ft);

power_spectrum=abs(Ftrns_acf)/N_ft;
plot(freq, power_spectrum(2:N_half+1));

%--------------------------------------------------------------------------------------------------


% Window generating
% half of simmetric window is needed for positive frequencies 
N_half_window=254
 for(i=1:1:N_half_window)
      half_window(i)=(1-abs(i-1)/(N_half_window-1));
end;

plot(-N_half_window:-1,half_window(N_half_window:-1:1),1:N_half_window,half_window)

% ACF by window multiplication
for(i=1:1:N_half_window)
 acf_w(i)=acf(i)*half_window(i);
end;

plot(acf_w);

%--------------------------------------------------------------------------------------------------

% Blackman-Tukey power spectrum calculation

Ftrns_acf_w=fft(acf_w,N_ft);

power_spectrum_w=abs(Ftrns_acf_w)/N_ft;
plot(freq, power_spectrum_w(2:N_half+1));
