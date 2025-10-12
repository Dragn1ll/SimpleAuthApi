-- Создание таблиц
create table genders (
    id serial primary key,
    name varchar(50) not null unique,
    code varchar(10) not null unique
);

create table roles (
    id serial primary key,
    name varchar(100) not null unique,
    description varchar(255),
    createddate timestamp not null default now()
);

create table users (
    id serial primary key,
    email varchar(255) not null unique,
    password varchar(255) not null,
    username varchar(100) not null unique,
    genderid int not null references genders(id),
    createddate date not null default now(),
    updateddate date not null default now(),
    isactive boolean not null default true,
    lastlogindate timestamp null
);

create table userroles (
    userid int not null references users(id) on delete cascade,
    roleid int not null references roles(id),
    assigneddate timestamp not null default now(),
    primary key (userid, roleid)
);

create table userprofiles (
    id serial primary key,
    userid int not null unique references users(id) on delete cascade,
    firstname varchar(100),
    lastname varchar(100),
    dateofbirth date null,
    phonenumber varchar(20),
    country varchar(100),
    city varchar(100),
    bio varchar(500),
    avatarurl varchar(255)
);


create table useractivitylogs (
    id bigserial primary key,
    userid int not null references users(id),
    activitytype varchar(50) not null,
    description varchar(500),
    ipaddress varchar(45),
    useragent varchar(500),
    createddate timestamp not null default now()
);

-- Добавление начальных данных

insert into genders (name, code) values
('Мужской', 'male'),
('Женский', 'female'),
('Другой', 'other');

insert into roles (name, description) values
('Администратор', 'Полный доступ ко всем функциям системы'),
('Модератор', 'Может управлять контентом и пользователями'),
('Пользователь', 'Обычный пользователь системы'),
('Гость', 'Ограниченный доступ');

insert into users (email, password, username, genderid, createddate, updateddate) values
('admin@example.com', 'hashed_password_1', 'admin_user', 1, '2023-01-15', '2024-01-10'),
('user1@example.com', 'hashed_password_2', 'john_doe', 1, '2023-03-20', '2024-01-12'),
('user2@example.com', 'hashed_password_3', 'jane_smith', 2, '2023-05-10', '2024-01-11'),
('user3@example.com', 'hashed_password_4', 'alex_brown', 3, '2023-07-05', '2024-01-09'),
('user4@example.com', 'hashed_password_5', 'sarah_jones', 2, '2023-09-18', '2024-01-08');

insert into userroles (userid, roleid) values
(1, 1),
(1, 2),
(2, 3),
(3, 3),
(4, 3),
(5, 3),
(5, 2);

insert into userprofiles (userid, firstname, lastname, dateofbirth, country, city) values
(1, 'Иван', 'Петров', '1985-03-15', 'Россия', 'Москва'),
(2, 'Джон', 'Доу', '1990-07-22', 'США', 'Нью-Йорк'),
(3, 'Джейн', 'Смит', '1992-11-05', 'Великобритания', 'Лондон'),
(4, 'Алекс', 'Браун', '1988-04-30', 'Канада', 'Торонто'),
(5, 'Сара', 'Джонс', '1995-09-14', 'Австралия', 'Сидней');

insert into useractivitylogs (userid, activitytype, description) values
(1, 'LOGIN', 'Успешный вход в систему'),
(2, 'REGISTER', 'Регистрация нового пользователя'),
(3, 'UPDATE_PROFILE', 'Обновление профиля'),
(4, 'LOGIN', 'Успешный вход в систему'),
(5, 'CHANGE_PASSWORD', 'Смена пароля');

-- Запросы
-- Пользователи с максимальной и минимальной датой регистрации
select 
    u.id,
    u.username,
    u.email,
    u.createddate,
    g.name as gender
from users u
inner join genders g on u.genderid = g.id
where u.createddate = (select min(createddate) from users where isactive = true);

select 
    u.id,
    u.username,
    u.email,
    u.createddate,
    g.name as gender
from users u
inner join genders g on u.genderid = g.id
where u.createddate = (select max(createddate) from users where isactive = true);

-- Количество мужчин и женщин
select 
    g.name as gender,
    count(u.id) as usercount,
    round(count(u.id) * 100.0 / (select count(*) from users where isactive = true), 2) as percentage
from users u
inner join genders g on u.genderid = g.id
where u.isactive = true
group by g.id, g.name
order by usercount desc;

select 
    g.name as gender,
    count(u.id) as totalcount,
    min(u.createddate) as firstregistration,
    max(u.createddate) as lastregistration,
    round(avg(extract(day from (now()::date - u.createddate))), 2) as avgdayssinceregistration
from users u
inner join genders g on u.genderid = g.id
where u.isactive = true
group by g.id, g.name;

-- Статистика по ролям
select 
    r.name as rolename,
    r.description,
    count(ur.userid) as usercount,
    round(count(ur.userid) * 100.0 / (select count(*) from users where isactive = true), 2) as percentageoftotalusers
from roles r
left join userroles ur on r.id = ur.roleid
left join users u on ur.userid = u.id and u.isactive = true
group by r.id, r.name, r.description
order by usercount desc;

select 
    r.name as rolename,
    count(ur.userid) as usercount,
    string_agg(u.username, ', ' order by u.username) as usersinrole
from roles r
left join userroles ur on r.id = ur.roleid
left join users u on ur.userid = u.id and u.isactive = true
group by r.id, r.name
order by usercount desc;

select 
    u.username,
    u.email,
    count(ur.roleid) as rolecount,
    string_agg(r.name, ', ' order by r.name) as roles
from users u
inner join userroles ur on u.id = ur.userid
inner join roles r on ur.roleid = r.id
where u.isactive = true
group by u.id, u.username, u.email
having count(ur.roleid) > 1
order by rolecount desc;
