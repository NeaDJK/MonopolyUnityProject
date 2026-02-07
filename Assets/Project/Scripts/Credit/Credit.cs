using System;
using Random = UnityEngine.Random;
using static System.Math;
using UnityEngine;

[System.Serializable]
public class Credit : MonoBehaviour
{
    // Сделаем поля public или добавим [SerializeField]
    [SerializeField] public int _sum;                // Сумма
    [SerializeField] public int _countOfSteps;       // Срок кредита (круги)
    [SerializeField] public int _percent;            // Процент
    [SerializeField] public double _coefPercent;     // Коэффициент
    [SerializeField] public int _typeOfCredit;       // Тип кредита
    [SerializeField] public int _payment;            // Платеж
    [SerializeField] public int _currentCircle;      // Текущий круг
    
    // Конструктор по умолчанию
    public Credit() { }
    
    // Конструктор копирования
    public Credit(Credit source)
    {
        if (source != null)
        {
            _sum = source._sum;
            _countOfSteps = source._countOfSteps;
            _percent = source._percent;
            _coefPercent = source._coefPercent;
            _typeOfCredit = source._typeOfCredit;
            _payment = source._payment;
            _currentCircle = source._currentCircle;
        }
    }
    
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
        _currentCircle = 0;
        CalculateCurrentPayment(1);
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

    // Добавление текущего круга
    public void AddCurrentCircle(int count = 1)
    {
        _currentCircle += count;
        EndCredit();
    }

    // Присвоение текущего круга
    public void SetCurrentCircle(int count)
    {
        _currentCircle = count;
        EndCredit();
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