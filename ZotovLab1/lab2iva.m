% Программа демонстрирует фильтрацию «скользящим средним»
clc; 
clear;
N=99;
f = mod([1:N],20)-5 + 5*randn([1,N]); % исходный сигнал
f(1:N/10) = 0;
f(9*N/10:N) = 0;

for window = [3, 7, 9, 11]

    h = zeros(size(f));
    
    if window == 11 % гауссов фильтр
        sigma = 3, mu = 11
        a = 1/(sigma*sqrt(2*pi)), b = mu, c = sigma
        t = [1:20]
        h(t) = a*exp((-(t-b).^2)/(2*c*c))
    else
        h(1: window) = 1/window;
    end
        h = circshift(h, (N-window)/2); % центрирование

    F = (fft(f));   % спектр исходного сигнала
    H = (fft(h));   % частотная хар-ка фильтра
    f2 = fftshift(ifft(F.*H));  % обратное преобр Фурье 

    hc = nonzeros(h)'
    f_ma = conv(f, hc); % свёртка сигнала с импульсной хар-кой
    f_ma = f_ma(1+window/2:end-window/2); % убрать лишнее

    tiledlayout(3,2)
    nexttile;
    plot(f);
    legend({'Исходный сигнал'},'Location','northwest')
    nexttile;
    hold on
    plot(abs(fftshift(F)), '--')
    plot(abs(fftshift(F.*H)))
    legend({'Спектр сигнала до фильтрации', 'Спектр после фильтрации'},'Location','northwest')
    hold off

    nexttile;
    plot(h)
    legend({'Импульсная х-ка фильтра'},'Location','southwest')
    nexttile;
    plot(abs(fftshift(H)))
    legend({'Частотная х-ка фильтра'},'Location','northwest')
   
    nexttile;
    hold on
    plot(f_ma, 'o');
    plot(f2, '--');
    legend({'Свёртка', 'Умнож спектров'},'Location','southwest')
    hold off
end

return
