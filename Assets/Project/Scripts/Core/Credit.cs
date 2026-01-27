using System;
using Random = UnityEngine.Random;
using static System.Math;
using UnityEngine;

public class Credit : MonoBehaviour
{
    private int _sum;                // Сумма
    private int _countOfSteps;       // Срок кредита (круги)
    private int _percent;            // Процент
    private double _coefPercent;     // Коэффициент
    private int _typeOfCredit;       // Тип кредита
    private int _payment;         // Платеж
    private int _currentCircle;      // Текущий круг
    
    // Проверка на закрытие кредита
    public void EndCredit()
    {
        if (_currentCircle > _countOfSteps)
        {
            ResetCredit();
        }
    }

    // Сброс кредита
    private void ResetCredit()
    {
        _sum = 0;
        _countOfSteps = 0;
        _percent = 0;
        _coefPercent = 1;
        _typeOfCredit = 0;
        _payment = 0;
        _currentCircle = 0;
    }

    // Получение суммы кредита
    public int GetSum()
    {
        return _sum;
    }

    // Создание нового кредитного плана
    public void NewCredit(int sMin = 1000, int sMax = 10000, int nMin = 3, int nMax = 10, int pMin = 11, int pMax = 15)
    {
        _sum = Random.Range(sMin, sMax + 1);
        _countOfSteps = Random.Range(nMin, nMax + 1);
        _percent = Random.Range(pMin, pMax + 1);
        _coefPercent = 1 + _percent / 100.0;
        _typeOfCredit = Random.Range(0, 2);
        CalculateCurrentPayment();
        _currentCircle = 0;
    }

    // Вычисление текущего платежа
    private void CalculateCurrentPayment(int i = 1)
    {
        if (_sum == 0)
        {
            _payment = 0;
        }
        else
        {
            if (_typeOfCredit == 0) // Аннуитетный платеж
            {
                _payment = (int) (_sum * Pow(_coefPercent, _countOfSteps) * (_coefPercent - 1) / (Pow(_coefPercent, _countOfSteps) - 1));
            }
            else // Дифференцированный платеж
            {
                if ((i <= _countOfSteps) && (i > 0))
                {
                    _payment = (int) (_sum * 1.0 / _countOfSteps * (_coefPercent * (_countOfSteps - i + 1) - (_countOfSteps - i)));
                }
                else
                {
                    _payment = -1;
                }
            }
        }
    }

    // Получение текущего платежа за круг
    public int GetCurrentPaymentForCircle()
    {
        CalculateCurrentPayment(_currentCircle);
        return _payment;
    }

    // Вывод информации по кредиту
    public string GetCreditInfo_ToString(int num = -1)
    {
        string info = "";

        if (num != -1)
        {
            info += $"Схема номер {num}\n";
        }

        if (_typeOfCredit == 0)
        {
            info += "Форма платежа: аннуитетная\n";
        }
        else
        {
            info += "Форма платежа: дифференцированная\n";
        }

        info += $"Сумма: {_sum}\n";
        info += $"Срок кредита: {_countOfSteps}\n";
        info += $"Процент: {_percent}\n";

        if (_typeOfCredit == 0)
        {
            CalculateCurrentPayment();
            info += $"Платеж: {_payment:F2}\n";
        }
        else
        {
            // Первый платеж
            CalculateCurrentPayment(1);
            info += $"Первый платеж: {_payment:F2}\n";
            
            // Последний платеж
            CalculateCurrentPayment(_countOfSteps);
            info += $"Последний платеж: {_payment:F2}\n";
            
            // Возвращаем расчет для текущего круга
            CalculateCurrentPayment(_currentCircle);
        }

        return info;
    }

    public Credit GetCreditInfo_ToCredit()
    {
        // Создаем новый объект Credit
        Credit cr = new Credit();
        
        // Копируем все поля из текущего объекта
        cr._sum = this._sum;
        cr._countOfSteps = this._countOfSteps;
        cr._percent = this._percent;
        cr._coefPercent = this._coefPercent;
        cr._typeOfCredit = this._typeOfCredit;
        
        // Расчет текущего платежа для копии
        cr.CalculateCurrentPayment(cr._currentCircle);
        
        return cr;
    }

    // Добавление текущего круга
    public void AddCurrentCircle(int count = 1)
    {
        _currentCircle += count;
    }

    // Присвоение текущего круга
    public void SetCurrentCircle(int count)
    {
        _currentCircle = count;
    }

    // Получение текущего круга
    public int GetCurrentCircle()
    {
        return _currentCircle;
    }

    // Получение срока кредита
    public int GetCountOfSteps()
    {
        return _countOfSteps;
    }

    // Получение типа кредита
    public int GetTypeOfCredit()
    {
        return _typeOfCredit;
    }

    // Получение процента
    public int GetPercent()
    {
        return _percent;
    }

    // Получение текущего платежа
    public double GetPayment()
    {
        return _payment;
    }
}
