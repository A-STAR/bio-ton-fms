Для локального запуска тестов необходимо:

1) Локально должен быть установлен Docker Desktop
2) Создать сеть, используя команду 
docker network create integration-tests
3) Создать контейнер rabbit-server из образа rabbitmq:3-management доступный на хосте через порты 15673 (веб итерфейс) и 5673 (api)
4) Добавить контейнер rabbit-server в сеть integration-tests при помощи команды 
docker network connect integration-tests rabbit-server
5) должны быть собраны docker образы приложений fms-tracker-message и fms-tracker-tcp-server с меткой latest
