% Фильтрация сигнала «фильтром Пантелеева»
clc; clear; clf('reset');
N=1024;
Fd = 10000; % частота дискретизации 10 кГц
Td = 1/Fd; % период дискретизации, c
ff = Fd*[-N/2+1:N/2]/N;
% соберём фильтр верхних частот с частотой среза 4 кГц
N2 = N/2 - 1;
w0 = 2*pi*4000;
w = 2*pi*Fd*[0:N2]/N2;
H = j*w ./ (j*w+w0);
H = H .* H;
H = [H flip(H)]; % ФВЧ 2-го порядка

f = 2*randn([1,N]); % чистый шум
F = fft(f) .* H; % отфильтруем шум
plot(abs(F)); % спектр шума

plot(f)
legend({'Белый шум'},'Location','northwest')
plot(ff, abs(fftshift(H)))
legend({'АЧХ чильтра'},'Location','northwest')

t=[0:N-1];
f = real(ifft(F)) + 0.3*sin(2*pi*t/20) + sin(2*2*pi*(t+20)/20);
plot(f(1:100))  % исходный сигнал 
legend({'Исходный сигнал '},'Location','northwest')
F = fft(f); % спектр сигнала с шумом
plot(ff, abs(fftshift(F))); 
legend({'Спектр сигнала с шумом'},'Location','northwest')

% способ 1. через импульсную характеристику фильтра
W0 = 2*pi*2000; % частота среза фильтра 2 кГц
td = Td * [-N/2+1:N/2];
W0 = W0/ sqrt(2);
h = W0 * exp(-W0 * abs(td)) .* (cos(W0 * td) + sin(W0 * abs(td))) / 2;
h = h / Fd;
plot(h(4*N/10:6*N/10))
legend({'Импльсная характеристика'},'Location','southwest')
H = fft(h);
plot(ff, abs(fftshift(H)))
legend({'АЧХ чильтра'},'Location','northwest')

% фильтрация фильтром Пантелеева сигнала с шумом 
F2 = F .* H;

plot(ff, abs(fftshift(F2)))
legend({'Спектр отфильтрованного сигнала'},'Location','northwest')

f2 = fftshift(ifft(F2));
plot(f2(1:300))

% способ 2. через свёртку
hc = nonzeros(h)';
f_ma = conv(f, hc); % свёртка сигнала с импульсной хар-кой
plot(f_ma(N/2:4*N/3));

