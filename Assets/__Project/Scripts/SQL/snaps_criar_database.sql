CREATE SCHEMA SNAPS;

use SNAPS;

CREATE TABLE DadosPesquisa (
    idUsuario   INT(10)                          NOT NULL auto_increment,
    sexo        VARCHAR(1)                       NOT NULL,
    idade       INT(3)                           NOT NULL DEFAULT 0,
    curso       VARCHAR(25)   CHARACTER SET utf8 NOT NULL DEFAULT 'Não informado',
    periodo     INT(4)                           NOT NULL DEFAULT 0,
    freq_estudo INT(1)                           NOT NULL,
    qualidade   INT(1)                           NOT NULL,
    comentarios VARCHAR(100)  CHARACTER SET utf8 NOT NULL DEFAULT 'Não informado',
    PRIMARY KEY (idUsuario)
);

CREATE TABLE Ranking (
    idRanking INT(11)                            NOT NULL auto_increment,
    iniciais VARCHAR(10)                         NOT NULL,
    pontos INT(4)                                NOT NULL DEFAULT 0,
    PRIMARY KEY (idRanking)
);