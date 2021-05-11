clear;

N_signal=1024;

noise=randn(1,N_signal);
noise1=50*randn(1,N_signal);

N_ft=N_signal;

Ftrns_noise=fft(noise,N_ft);

plot(abs(Ftrns_noise)/N_ft)

a=4

dt=1;

for(k=1:1:N_ft)
      if (k==1)
          P(k)=0;
          omega(k)=0; 
          W(k)=1/a;
          
      elseif(k<=N_ft/2+1)
          P(k)=N_ft*dt/(k-1);
          omega(k)=2*pi/P(k); 
          W(k)=1/(a-omega(k)^2);
         
      elseif(k>N_ft/2+1)
          P(k)=-N_ft*dt/(N_ft-k+1);
          omega(k)=2*pi/P(k);  
          W(k)=1/(a-omega(k)^2);
         
          
      end;
 end;

Ftrns_x=Ftrns_noise.*W;

plot(P,10000*abs(Ftrns_noise)/N_ft,P,abs(W),P,Ftrns_x)



X=ifft(Ftrns_x);

signal=X+noise1;
plot(signal)

signal_centered=signal-mean(signal)

for(tau=1:1:N_signal)
 acf(tau)=0;
 for(j=1:1:N_signal-tau)
    acf(tau)=acf(tau)+signal_centered(j)*signal_centered(j+tau-1);
 end;
  acf(tau)= acf(tau)/(N_signal);
end;
plot(acf);


Ftrns_acf_signal=fft(acf,N_ft);


for(tau=1:1:N_signal)
 acf(tau)=0;
 for(j=1:1:N_signal-tau)
    acf(tau)=acf(tau)+noise(j)*noise(j+tau-1);
 end;
  acf(tau)= acf(tau)/(N_signal);
end;
plot(acf);


Ftrns_acf_noise=fft(acf,N_ft);


plot(P,abs(Ftrns_acf_signal)./abs(Ftrns_acf_noise),P,W.*conj(W))