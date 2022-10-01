create table UidWorkerNode
(
    Id         bigint auto_increment
        primary key,
    HostName   varchar(100) charset utf8mb3 default '''' not null,
    Ip         varchar(100) default '''' not null,
    Type       int          default 0    not null,
    LaunchDate timestamp    default CURRENT_TIMESTAMP null
);
alter table UidWorkerNode auto_increment=1025;