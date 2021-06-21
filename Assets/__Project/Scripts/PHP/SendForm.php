<?php
    $connection = mysqli_connect('forro.ufla.br', 'admin_bdgdl', 'BdHrow394', 'bd_gdl');

    if (mysqli_connect_errno()) {
        exit();
    }

    $sexo = $_POST["sexo"];
    $idade = $_POST["idade"];
    $curso = $_POST["curso"];
    $periodo = $_POST["periodo"];
    $frequencia = $_POST["frequencia_estudo"];
    $qualidade = $_POST["qualidade_jogo"];
    $comentarios = $_POST["comentarios"];

    $query = "INSERT INTO dadospesquisa (`idUsuario`, `sexo`, `idade`, `curso`, `periodo`,
                `freq_estudo`, `qualidade`, `comentarios`)  

                VALUES (NULL, '". $sexo ."', '". $idade ."', '".$curso."', '".$periodo."',
                 '".$frequencia."', '".$qualidade."', '".$comentarios."');";

    $result = mysqli_query($connection, $query) or die();

    mysqli_close($connection);
?>