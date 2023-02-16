namespace TestTcpTrackerClient;

public static class Constants
{
    public const string Instruction = @"Варианты запуска приложения:
- C двумя параметрами <message_file> <result_file>.
В этом случае на сервер отправляется пакет данных из файла <message_file> и сохраняет результаты в файле <result_file>.
Если файл результата уже существует, он перезаписывается. 
- C тремя параметрами <message_file> <result_file> <repeat_count>.
В этом случае на сервер отправляется пакет данных из файла <message_file>, отправка повторяется <repeat_count> раз.
Результаты сохраняются в файлы <result_file1>…<result_fileN>.
Если файлы результата уже существуют, они перезаписываются.
- C одним параметром <script_file>.
В этом случае на сервер отправляется пакеты данных из файлов описанных в скрипте <script_file>.
Каждая строка файла <script_file> имеет формат message_file result_file, где message_file - это имя файла с сообщением, result_file - имя файла в который будет записан ответ на это сообщение.";
}