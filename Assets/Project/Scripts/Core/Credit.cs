using System;
using Random = UnityEngine.Random;
using static System.Math;
using UnityEngine;

public class Credit : MonoBehaviour
{
    public int _sum;                // Сумма
    public int _countOfSteps;       // Срок кредита (круги)
    public int _percent;            // Процент
    public double _coefPercent;     // Коэффициент
    public int _typeOfCredit;       // Тип кредита
    public int _payment;            // Платеж
    public int _currentCircle;      // Текущий круг (сколько уже выплачено)
    
    // Проверка на закрытие кредита
    public void EndCredit()
    {
        if (_currentCircle >= _countOfSteps)
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
        _currentCircle = 0; // Сбрасываем при создании нового кредита
        CalculateCurrentPayment(1); // Считаем первый платеж
    }

    // Вычисление платежа для конкретного круга (i - номер круга, начиная с 1)
    public void CalculateCurrentPayment(int circleNumber = 1)
    {
        if (_sum == 0 || _currentCircle >= _countOfSteps)
        {
            _payment = 0;
            return;
        }

        if (circleNumber < 1 || circleNumber > _countOfSteps)
        {
            _payment = -1;
            return;
        }

        if (_typeOfCredit == 0) // Аннуитетный платеж
        {
            _payment = (int)(_sum * Pow(_coefPercent, _countOfSteps) * (_coefPercent - 1) / (Pow(_coefPercent, _countOfSteps) - 1));
        }
        else // Дифференцированный платеж
        {
            _payment = (int)(_sum * 1.0 / _countOfSteps * (_coefPercent * (_countOfSteps - circleNumber + 1) - (_countOfSteps - circleNumber)));
        }
    }

    // Получение платежа для следующего круга
    public int GetNextPayment()
    {
        // Платеж для следующего круга (текущий + 1)
        int nextCircle = _currentCircle + 1;
        if (nextCircle > _countOfSteps) return 0;
        
        CalculateCurrentPayment(nextCircle);
        return _payment;
    }

    // Выплата кредита за текущий круг
    public int PayForCurrentCircle()
    {
        // Если кредит уже выплачен
        if (_currentCircle >= _countOfSteps)
        {
            ResetCredit();
            return 0;
        }

        // Получаем платеж для следующего круга
        int payment = GetNextPayment();
        
        // Увеличиваем счетчик выплаченных кругов
        _currentCircle++;
        
        // Проверяем, не закрыт ли кредит
        EndCredit();
        
        return payment;
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
        info += $"Срок кредита: {_countOfSteps} кругов\n";
        info += $"Процент: {_percent}%\n";
        info += $"Выплачено кругов: {_currentCircle}\n";
        info += $"Осталось кругов: {_countOfSteps - _currentCircle}\n";

        if (_currentCircle < _countOfSteps)
        {
            int nextPayment = GetNextPayment();
            info += $"Следующий платеж: {nextPayment}\n";
        }
        else
        {
            info += "Кредит полностью выплачен!\n";
        }

        return info;
    }

    // Создание копии кредита
    public Credit Clone()
    {
        Credit copy = new Credit();
        copy._sum = this._sum;
        copy._countOfSteps = this._countOfSteps;
        copy._percent = this._percent;
        copy._coefPercent = this._coefPercent;
        copy._typeOfCredit = this._typeOfCredit;
        copy._currentCircle = this._currentCircle;
        copy.CalculateCurrentPayment(copy._currentCircle + 1);
        
        return copy;
    }

    // Добавление текущего круга
    public void AddCurrentCircle(int count = 1)
    {
        _currentCircle += count;
        EndCredit(); // Проверяем, не закрыт ли кредит
    }

    // Присвоение текущего круга
    public void SetCurrentCircle(int count)
    {
        _currentCircle = count;
        EndCredit(); // Проверяем, не закрыт ли кредит
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
    public int GetPayment()
    {
        return _payment;
    }

    // Проверка, активен ли кредит
    public bool IsActive()
    {
        return _sum > 0 && _currentCircle < _countOfSteps;
    }
}